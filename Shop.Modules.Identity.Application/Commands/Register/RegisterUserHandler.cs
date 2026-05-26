using MediatR;
using Microsoft.AspNetCore.Identity;
using Modules.Identity.Application.Interfaces;
using Modules.Identity.Domain;
using Shared.Domain;

namespace Modules.Identity.Application.Commands.Register;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly UserManager<User> _userManager; 
    private readonly IAppIdentityDbContext _context;

    public RegisterUserHandler(UserManager<User> userManager, IAppIdentityDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result<RegisterUserResponse>.Failure("Пользователь с таким Email уже существует");

        var userResult = User.Create(request.FirstName, request.LastName, request.Email);
        if (userResult.IsFailure)
            return Result<RegisterUserResponse>.Failure(userResult.Error!);

        var user = userResult.Value!;

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if(!identityResult.Succeeded)
        {
            var error = identityResult.Errors.First().Description;
            return Result<RegisterUserResponse>.Failure(error);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<RegisterUserResponse>.Success(new RegisterUserResponse(user.Id, user.Email!));
    }
}