namespace Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductResponse(int Id, string Name, decimal PriceAmount, string PriceCurrency, string? ImageUrl);