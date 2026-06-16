using MediatR;
using Shared.Domain;

namespace Catalog.Application.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int Id) : IRequest<Result>;