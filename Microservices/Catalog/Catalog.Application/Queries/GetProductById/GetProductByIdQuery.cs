using MediatR;
using Shared.Domain;

namespace Catalog.Application.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<GetProductByIdResponse>>;