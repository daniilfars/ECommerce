namespace Modules.Identity.Application.Commands.Register;

public sealed record RegisterUserResponse(Guid UserId, string Email);