using MediatR;
using Modules.Basket.Application.Queries.GetBasket;
using Modules.Basket.Domain;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.UpdateBasketItemQuantity;

public class UpdateBasketItemQuantityHandler : IRequestHandler<UpdateBasketItemQuantityCommand, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;

    public UpdateBasketItemQuantityHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<GetBasketResponse>> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            basket = Domain.Basket.Create(request.UserId);

        var result = basket.UpdateQuantity(request.ProductId, request.Quantity);
        if (result.IsFailure)
            return Result<GetBasketResponse>.Failure(result.Error!);

        await _basketRepository.SaveBasketAsync(basket);

        return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.PriceAmount, i.PriceCurrency, i.Quantity, i.TotalPrice, i.ImageUrl)).ToList(), basket.TotalAmount));
    }
}