using MediatR;
using Shared.Domain;

namespace Modules.Identity.Application.Commands.Logout;

public sealed record LogoutUserCommand() : IRequest<Result>;