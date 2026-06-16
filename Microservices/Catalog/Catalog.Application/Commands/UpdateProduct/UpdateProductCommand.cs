using MediatR;
using Shared.Domain;

namespace Catalog.Application.Commands.UpdateProduct;

public sealed record UpdateProductCommand(int Id, string? Name = null, decimal? PriceAmount = null, string? PriceCurrency = null) : IRequest<Result<UpdateProductResponse>>;