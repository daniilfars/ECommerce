using MediatR;
using Modules.Basket.Application.Queries.GetBasket;
using Modules.Basket.Domain;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketHandler : IRequestHandler<RemoveItemFromBasketCommand, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<GetBasketResponse>> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, new List<BasketItemDto>(), 0));

        var result = basket.RemoveItem(request.ProductId);
        if (result.IsFailure)
            return Result<GetBasketResponse>.Failure(result.Error!);

        await _basketRepository.SaveBasketAsync(basket);

        return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.PriceAmount, i.PriceCurrency, i.Quantity, i.TotalPrice, i.ImageUrl)).ToList(), basket.TotalAmount));
    }
}