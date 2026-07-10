using MassTransit;
using Shared.Contracts;
using StackExchange.Redis;

namespace Basket.Application.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeleted>
{
    private readonly IDatabase _redisDb;

    public ProductDeletedConsumer(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        await _redisDb.KeyDeleteAsync($"product:{context.Message.ProductId}:stock");
    }
}