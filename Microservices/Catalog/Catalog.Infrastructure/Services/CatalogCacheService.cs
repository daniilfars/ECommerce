using Catalog.Application.Interfaces;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Services;

public class CatalogCacheService : ICatalogCacheService
{
    private readonly IDatabase _cache;
    private readonly IConnectionMultiplexer _multiplexer;

    public CatalogCacheService(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
        _cache = multiplexer.GetDatabase();
    }

    public async Task ClearCatalogPagesAsync()
    {
        var endpoints = _multiplexer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _multiplexer.GetServer(endpoint);
            await foreach (var key in server.KeysAsync(pattern: "catalog:page:*"))
            {
                await _cache.KeyDeleteAsync(key);
            }
        }
    }

    public async Task ClearProductByIdAsync(int productId)
    {
        var key = $"catalog:product:{productId}";
        await _cache.KeyDeleteAsync(key);
    }
}
