using MediatR;
using System.Net.Http.Json;
using Basket.Domain;
using Shared.Domain;
using Basket.Application.Models;

namespace Basket.Application.Commands.CheckoutBasket;

public class CheckoutBasketHandler : IRequestHandler<CheckoutBasketCommand, Result>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public CheckoutBasketHandler(IBasketRepository basketRepository, IHttpClientFactory httpClientFactory)
    {
        _basketRepository = basketRepository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            return Result.Failure("Нельзя сделать заказ пустой корзины");

        var createOrderCommand = new CreateOrderCommand(basket.UserId, request.ShippingAddress,
            basket.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.ProductName,
                i.PriceAmount,
                i.PriceCurrency,
                i.Quantity,
                i.ImageUrl
            )).ToList()
        );

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("http://ordering-api/api/Ordering/create", createOrderCommand, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Result.Failure(error);
        }

        await _basketRepository.DeleteAsync(request.UserId);
        return Result.Success();
    }
}