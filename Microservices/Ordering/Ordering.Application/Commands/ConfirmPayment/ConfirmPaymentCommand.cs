using MediatR;
using Shared.Domain;

public sealed record ConfirmPaymentCommand(int OrderId, Guid UserId, bool IsAdmin) : IRequest<Result>;