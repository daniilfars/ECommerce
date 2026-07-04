using MediatR;
using Shared.Domain;

namespace Ordering.Application.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId, Guid UserId, bool IsAdmin = false) : IRequest<Result>;