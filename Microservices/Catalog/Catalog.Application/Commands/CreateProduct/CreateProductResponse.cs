namespace Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductResponse(int Id, string Name, decimal Price, string? ImageUrl);