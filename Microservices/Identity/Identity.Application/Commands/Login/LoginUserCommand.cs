using MediatR;
using Shared.Domain;

namespace Identity.Application.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginUserResponse>>;