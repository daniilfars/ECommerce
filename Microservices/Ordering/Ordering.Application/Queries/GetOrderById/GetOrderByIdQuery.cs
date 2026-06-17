using MediatR;
using Shared.Domain;

namespace Ordering.Application.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(int OrderId, Guid UserId, bool IsAdmin = false) : IRequest<Result<GetOrderByIdResponse>>;
