using MediatR;
using Shared.Domain;
using Modules.Basket.Domain;

namespace Modules.Basket.Application.Queries.GetBasket;

public class GetBasketHandler : IRequestHandler<GetBasketQuery, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;

    public GetBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<GetBasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, new List<BasketItemDto>(), 0));

        return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.PriceAmount, i.PriceCurrency, i.Quantity, i.TotalPrice)).ToList(), basket.TotalAmount));
    }
}