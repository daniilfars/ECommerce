using MediatR;
using Shared.Domain;

namespace Ordering.Application.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId, Guid UserId, bool IsAdmin = false) : IRequest<Result<GetOrderByIdResponse>>;
