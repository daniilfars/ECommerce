using MediatR;
using Microsoft.Extensions.Configuration;
using Ordering.Application.Interfaces;
using Shared.Domain;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand, Result>
{
    private readonly IOrderingDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ConfirmPaymentHandler(IOrderingDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        if (!request.IsAdmin && order.UserId != request.UserId)
            return Result.Failure("Нет доступа к заказу");

        if (order.PaymentId is null)
            return Result.Failure("Платёж не создан");

        var shopId = _configuration["Yookassa:ShopId"]!;
        var secretKey = _configuration["Yookassa:SecretKey"]!;

        var client = _httpClientFactory.CreateClient();
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shopId}:{secretKey}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var response = await client.GetAsync($"https://api.yookassa.ru/v3/payments/{order.PaymentId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return Result.Failure("Не удалось проверить статус платежа");

        var paymentData = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        var status = paymentData.GetProperty("status").GetString()!;

        if (status != "succeeded")
            return Result.Failure("Платёж не оплачен");

        var result = order.Pay();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}