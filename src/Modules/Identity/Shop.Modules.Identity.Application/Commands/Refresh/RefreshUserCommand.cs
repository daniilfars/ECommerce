using MediatR;
using Shared.Domain;

namespace Modules.Identity.Application.Commands.Refresh;

public sealed record RefreshUserCommand() : IRequest<Result<RefreshUserResponse>>;