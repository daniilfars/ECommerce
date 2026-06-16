using MediatR;
using Shared.Domain;

namespace Identity.Application.Commands.Logout;

public sealed record LogoutUserCommand() : IRequest<Result>;