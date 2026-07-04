using MediatR;
using Shared.Domain;

namespace Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal Price, int StockQuantity) : IRequest<Result<CreateProductResponse>>;