namespace Catalog.Application.Queries.GetProductById;

public sealed record GetProductByIdResponse(int Id, string Name, decimal Price, string? ImageUrl);