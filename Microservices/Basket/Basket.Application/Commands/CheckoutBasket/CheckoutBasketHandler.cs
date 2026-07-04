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
    private readonly IPublishEndpoint _publishEndpoint;

    public CheckoutBasketHandler(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint)
    {
        _basketRepository = basketRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            return Result.Failure("Нельзя сделать заказ пустой корзины");

        await _publishEndpoint.Publish<OrderCreated>(new
        {
            OrderId = Guid.NewGuid(),
            UserId = request.UserId,
            ShippingAddress = request.ShippingAddress,
            TotalAmount = basket.TotalAmount,
            Items = basket.Items.Select(i => new
            {
                i.ProductId,
                i.ProductName,
                i.Price,
                i.Quantity,
                i.ImageUrl
            }).ToArray()
        });

        await _basketRepository.DeleteAsync(request.UserId);
        return Result.Success();
    }
}