using MediatR;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int Id) : IRequest<Result>;