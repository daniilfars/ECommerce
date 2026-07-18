using MediatR;
using Shared.Domain;

namespace Catalog.Application.Queries.SearchProducts;

public sealed record SearchProductsQuery(string SearchTerm, int Page = 1, int PageSize = 10, decimal? MinPrice = null, decimal? MaxPrice = null) : IRequest<Result<SearchProductsResponse>>;