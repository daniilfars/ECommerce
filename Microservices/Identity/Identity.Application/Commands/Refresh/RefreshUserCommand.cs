using MediatR;
using Shared.Domain;

namespace Identity.Application.Commands.Refresh;

public sealed record RefreshUserCommand() : IRequest<Result<RefreshUserResponse>>;