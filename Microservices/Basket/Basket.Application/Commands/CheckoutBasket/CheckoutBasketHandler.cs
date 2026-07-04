using Basket.Application.Models;
using Basket.Domain;
using MediatR;
using Shared.Domain;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using MassTransit;
using Shared.Contracts;

namespace Basket.Application.Commands.CheckoutBasket;

public class CheckoutBasketHandler : IRequestHandler<CheckoutBasketCommand, Result>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public CheckoutBasketHandler(IBasketRepository basketRepository, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint)
    {
        _basketRepository = basketRepository;
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _publishEndpoint = publishEndpoint;
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
                i.Price,
                i.Quantity,
                i.ImageUrl
            )).ToList()
        );

        //await _publishEndpoint.Publish<OrderCreated>(new {});

        var token = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString();

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

        var response = await client.PostAsJsonAsync("http://ordering-api:8080/api/Ordering/create", createOrderCommand, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Result.Failure(error);
        }

        await _basketRepository.DeleteAsync(request.UserId);
        return Result.Success();
    }
}