using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Inference;
using MassTransit;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeleted>
{
    private readonly ElasticsearchClient _elasticClient;

    public ProductDeletedConsumer(ElasticsearchClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        var response = await _elasticClient.DeleteAsync<ProductDeletedConsumer>(context.Message.ProductId, idx => idx.Index("products"));

        if(!response.IsValidResponse)
            throw new Exception($"Не удалось удалить товар {context.Message.ProductId} из Elasticsearch");
    }
}
