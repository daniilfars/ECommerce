using Catalog.Application.Models;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
{
    private readonly ElasticsearchClient _elasticClient;

    public ProductUpdatedConsumer(ElasticsearchClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task Consume(ConsumeContext<ProductUpdated> context)
    {
        var message = context.Message;

        var document = new ProductSearchDocument
        {
            Id = message.Id,
            Name = message.Name,
            ImageUrl = message.ImageUrl,
            Price = message.Price,
            StockQuantity = message.StockQuantity
        };

        var response = await _elasticClient.IndexAsync(document, idx => idx.Index("product").Id(document.Id));
        if (!response.IsValidResponse)
            throw new Exception($"Не удалось обновить товар {message.Id} в Elasticsearch");
    }
}