using MediatR;
using Shared.Domain;

public sealed record ConfirmPaymentCommand(int OrderId) : IRequest<Result>;