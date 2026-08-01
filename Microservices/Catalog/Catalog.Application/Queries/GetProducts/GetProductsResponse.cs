namespace Catalog.Application.Queries.GetProducts;

public sealed record GetProductsResponse(List<ProductDto> Products, int TotalCount, int Page, int PageSize);

public sealed record ProductDto(int Id, string Name, decimal Price, int StockQuantity, string? ImageUrl, decimal AverageStars, int ReviewCount);