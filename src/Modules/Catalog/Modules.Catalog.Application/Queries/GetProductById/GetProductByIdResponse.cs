namespace Modules.Catalog.Application.Queries.GetProductById;

public sealed record GetProductByIdResponse(int Id, string Name, decimal PriceAmount, string PriceCurrency, string? ImageUrl);