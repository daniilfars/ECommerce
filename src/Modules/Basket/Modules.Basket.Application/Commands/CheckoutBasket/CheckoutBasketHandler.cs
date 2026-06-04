using MediatR;
using Modules.Basket.Application.Queries.GetBasket;
using Modules.Basket.Domain;
using Modules.Ordering.Application.Commands.CreateOrder;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.CheckoutBasket;

public class CheckoutBasketHandler : IRequestHandler<CheckoutBasketCommand, Result>
{
    private readonly IMediator _mediator;
    private readonly IBasketRepository _basketRepository;

    public CheckoutBasketHandler(IMediator mediator, IBasketRepository basketRepository)
    {
        _mediator = mediator;
        _basketRepository = basketRepository;
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

        var result = await _mediator.Send(createOrderCommand);
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _basketRepository.DeleteAsync(request.UserId);
        return Result.Success();
    }
}
