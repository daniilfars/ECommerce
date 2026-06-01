using MediatR;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.DeliverOrder;

public sealed record DeliverOrderCommand(int OrderId) : IRequest<Result>;