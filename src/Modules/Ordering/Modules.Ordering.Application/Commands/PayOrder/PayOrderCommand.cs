using MediatR;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.PayOrder;

public sealed record PayOrderCommand(int OrderId, Guid UserId, bool IsAdmin = false) : IRequest<Result>;