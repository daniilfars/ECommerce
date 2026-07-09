using MediatR;
using Shared.Domain;
using Basket.Domain;
using StackExchange.Redis;

namespace Basket.Application.Queries.GetBasket;

public class GetBasketHandler : IRequestHandler<GetBasketQuery, Result<BasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IDatabase _redisDb;

    public GetBasketHandler(IBasketRepository basketRepository, IDatabase redisDb)
    {
        _basketRepository = basketRepository;
        _redisDb = redisDb;
    }

    public async Task<Result<BasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            return Result<BasketResponse>.Success(new BasketResponse(request.UserId, new List<BasketResponseDto>(), 0));

        var keys = basket.Items.Select(i => (RedisKey)$"product:{i.ProductId}:stock").ToArray();
        var cachedStocks = await _redisDb.StringGetAsync(keys);

        var itemsDto = basket.Items.Select((i, index) =>
        {
            var cachedStock = cachedStocks[index];
            int availableStock = cachedStock.HasValue ? (int)cachedStock : 0;

            return new BasketResponseDto(i.ProductId, i.ProductName, i.Price, i.Quantity, i.TotalPrice, i.ImageUrl, availableStock);
        }).ToList();

        return Result<BasketResponse>.Success(new BasketResponse(request.UserId, itemsDto, basket.TotalAmount));
    }
}