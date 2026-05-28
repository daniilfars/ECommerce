using MediatR;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal PriceAmount, string PriceCurrency) : IRequest<Result<CreateProductResponse>>;