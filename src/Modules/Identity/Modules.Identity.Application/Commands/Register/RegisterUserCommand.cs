using MediatR;
using Shared.Domain;

namespace Modules.Identity.Application.Commands.Register;

public sealed record RegisterUserCommand(string FirstName, string LastName, string Email, string Password) : IRequest<Result<RegisterUserResponse>>;