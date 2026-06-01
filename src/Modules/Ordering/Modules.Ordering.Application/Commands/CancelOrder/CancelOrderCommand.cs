using MediatR;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.CancelOrder;

public sealed record CancelOrderCommand(int OrderId, Guid UserId) : IRequest<Result>;
