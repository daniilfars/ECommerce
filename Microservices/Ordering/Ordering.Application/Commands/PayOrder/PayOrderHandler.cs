using MediatR;
using Microsoft.Extensions.Configuration;
using Ordering.Application.Interfaces;
using Shared.Domain;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Ordering.Application.Commands.PayOrder;

public class PayOrderHandler : IRequestHandler<PayOrderCommand, Result<PayOrderResponse>>
{
    private readonly IOrderingDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PayOrderHandler(IOrderingDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Result<PayOrderResponse>> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result<PayOrderResponse>.Failure("Заказ не найден");

        if (!request.IsAdmin && order.UserId != request.UserId)
            return Result<PayOrderResponse>.Failure("Нет доступа к заказу");

        if (order.Status != Domain.OrderStatus.Pending)
            return Result<PayOrderResponse>.Failure("Заказ нельзя оплатить");

        var shopId = _configuration["Yookassa:ShopId"]!;
        var secretKey = _configuration["Yookassa:SecretKey"]!;

        var client = _httpClientFactory.CreateClient();

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shopId}:{secretKey}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        client.DefaultRequestHeaders.Add("Idempotence-Key", Guid.NewGuid().ToString());

        var frontendUrl = _configuration["Frontend:Url"] ?? "http://localhost:55779";

        var response = await client.PostAsJsonAsync("https://api.yookassa.ru/v3/payments", new
        {
            amount = new {
                value = order.TotalAmount.ToString("F2"),
                currency = "RUB"
            },
            confirmation = new {
                type = "redirect",
                return_url = $"{frontendUrl}/orders/{order.Id}"
            },
            capture = true,
            description = $"Заказ {order.Id}"
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<PayOrderResponse>.Failure($"Ошибка создания платежа: {error}");
        }

        var paymentData = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        var paymentId = paymentData.GetProperty("id").GetString()!;
        var confirmationUrl = paymentData.GetProperty("confirmation").GetProperty("confirmation_url").GetString()!;

        order.SetPaymentId(paymentId);

        await _context.SaveChangesAsync(cancellationToken);
        return Result<PayOrderResponse>.Success(new PayOrderResponse(confirmationUrl));
    }
}