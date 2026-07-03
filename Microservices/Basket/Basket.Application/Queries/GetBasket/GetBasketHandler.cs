using MediatR;
using Shared.Domain;
using Basket.Domain;

namespace Basket.Application.Queries.GetBasket;

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

        return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.Price, i.Quantity, i.TotalPrice, i.ImageUrl)).ToList(), basket.TotalAmount));
    }
}