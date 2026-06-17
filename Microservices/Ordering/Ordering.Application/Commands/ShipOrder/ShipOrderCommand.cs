using MediatR;
using Shared.Domain;

namespace Ordering.Application.Commands.ShipOrder;

public sealed record ShipOrderCommand(int OrderId) : IRequest<Result>;