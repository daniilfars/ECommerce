using MediatR;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.CancelOrder;

public sealed record CancelOrderCommand(int OrderId, Guid UserId, bool IsAdmin = false) : IRequest<Result>;
