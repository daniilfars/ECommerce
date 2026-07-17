using Catalog.Application.Models;
using Elastic.Clients.Elasticsearch;
using MediatR;
using Shared.Domain;

namespace Catalog.Application.Queries.SearchProducts;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, Result<SearchProductsResponse>>
{
    private readonly ElasticsearchClient _elasticClient;

    public SearchProductsHandler(ElasticsearchClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task<Result<SearchProductsResponse>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return Result<SearchProductsResponse>.Success(new SearchProductsResponse(new List<ProductSearchDocument>(), 0, request.Page, request.PageSize));

        var response = await _elasticClient.SearchAsync<ProductSearchDocument>(s => s
            .Indices("products")
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(request.SearchTerm)
                    .Fuzziness(new Fuzziness("AUTO"))
                )
            ), cancellationToken);

        if (!response.IsValidResponse)
            return Result<SearchProductsResponse>.Failure($"Elastic Error: {response.DebugInformation}");

        var products = response.Documents.ToList();
        var totalCount = response.Total;

        var resultResponse = new SearchProductsResponse(products, (int)totalCount, request.Page, request.PageSize);

        return Result<SearchProductsResponse>.Success(resultResponse);
    }
}
