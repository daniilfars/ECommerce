using MediatR;
using Shared.Domain;

namespace Catalog.Application.Commands.UpdateProduct;

public sealed record UpdateProductCommand(int Id, string? Name = null, decimal? Price = null, int? StockQuantity = null) : IRequest<Result<UpdateProductResponse>>;