using MassTransit;
using Shared.Contracts;
using StackExchange.Redis;

namespace Basket.Application.Consumers;

public class ProductsStockChangedConsumer : IConsumer<ProductsStockChanged>
{
    private readonly IDatabase _redisDb;

    public ProductsStockChangedConsumer(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public async Task Consume(ConsumeContext<ProductsStockChanged> context)
    {
        var products = context.Message.Products;

        if (products == null || !products.Any())
            return;

        var transaction = _redisDb.CreateTransaction();

        foreach (var product in products)
        {
            var key = $"product:{product.ProductId}:stock";
            _ = transaction.StringSetAsync(key, product.StockQuantity, TimeSpan.FromDays(2));
        }

        await transaction.ExecuteAsync();
    }
}