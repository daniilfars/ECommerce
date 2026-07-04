using MediatR;
using Shared.Domain;

public sealed record ConfirmPaymentCommand(Guid OrderId, Guid UserId, bool IsAdmin) : IRequest<Result>;