using Modules.Basket.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Modules.Basket.Infrastructure;

public class RedisBasketRepository : IBasketRepository
{
    private readonly IDistributedCache _cache;

    public RedisBasketRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _cache.RemoveAsync(GetKey(userId));
    }

    public async Task<BasketEntity?> GetBasketAsync(Guid userId)
    {
        BasketEntity? basket = null;

        var basketString = await _cache.GetStringAsync(GetKey(userId));
        if (basketString != null)
            basket = JsonSerializer.Deserialize<BasketEntity>(basketString);

        return basket;
    }

    public async Task SaveBasketAsync(BasketEntity basket)
    {
        var basketString = JsonSerializer.Serialize(basket);

        await _cache.SetStringAsync(GetKey(basket.UserId), basketString, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        });
    }

    private static string GetKey(Guid userId) => $"basket:{userId}";
}