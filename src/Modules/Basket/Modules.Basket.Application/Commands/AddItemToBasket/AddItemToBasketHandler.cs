using MediatR;
using Modules.Basket.Application.Queries.GetBasket;
using Modules.Basket.Domain;
using Modules.Catalog.Application.Queries.GetProductById;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.AddItemToBasket;

public class AddItemToBasketHandler : IRequestHandler<AddItemToBasketCommand, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMediator _mediator;

    public AddItemToBasketHandler(IBasketRepository basketRepository, IMediator mediator)
    {
        _basketRepository = basketRepository;
        _mediator = mediator;
    }

    public async Task<Result<GetBasketResponse>> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var productResult = await _mediator.Send(new GetProductByIdQuery(request.ProductId));
        if (productResult.IsFailure)
            return Result<GetBasketResponse>.Failure("Товар не найден");

        var product = productResult.Value!;

        var itemResult = BasketItem.Create(product.Id, product.Name, product.PriceAmount, product.PriceCurrency, request.Quantity);
        if (itemResult.IsFailure)
            return Result<GetBasketResponse>.Failure(itemResult.Error!);

        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            basket = Domain.Basket.Create(request.UserId);

        var item = itemResult.Value!;

        basket.AddItem(item);

        await _basketRepository.SaveBasketAsync(basket);

        return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.PriceAmount, i.PriceCurrency, i.Quantity, i.TotalPrice)).ToList(), basket.TotalAmount));
    }
}