using MediatR;
using Shared.Domain;

namespace Ordering.Application.Commands.ShipOrder;

public sealed record ShipOrderCommand(Guid OrderId) : IRequest<Result>;