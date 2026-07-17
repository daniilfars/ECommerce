using Catalog.Application.Models;

namespace Catalog.Application.Queries.SearchProducts;

public sealed record SearchProductsResponse(List<ProductSearchDocument> Products, int TotalCount, int Page, int PageSize);