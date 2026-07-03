namespace Catalog.Application.Commands.UpdateProduct;

public sealed record UpdateProductResponse(int Id, string Name, decimal Price, string? ImageUrl);