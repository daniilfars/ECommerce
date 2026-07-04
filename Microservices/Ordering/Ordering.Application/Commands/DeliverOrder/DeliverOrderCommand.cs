using MediatR;
using Shared.Domain;

namespace Ordering.Application.Commands.DeliverOrder;

public sealed record DeliverOrderCommand(Guid OrderId) : IRequest<Result>;