using MediatR;
using Shared.Domain;

namespace Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal Price) : IRequest<Result<CreateProductResponse>>;