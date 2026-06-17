using MediatR;
using Shared.Domain;

namespace Ordering.Application.Commands.DeliverOrder;

public sealed record DeliverOrderCommand(int OrderId) : IRequest<Result>;