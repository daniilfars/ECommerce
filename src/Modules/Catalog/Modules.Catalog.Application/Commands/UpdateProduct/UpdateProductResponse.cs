namespace Modules.Catalog.Application.Commands.UpdateProduct;

public sealed record UpdateProductResponse(int Id, string Name, decimal PriceAmount, string PriceCurrency);