using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Modules.Identity.Application.Commands.Register;
using Modules.Identity.Application.Interfaces;
using Modules.Identity.Domain;
using Shared.Domain;

namespace Modules.Identity.Application.Commands.Login;

public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly IAppIdentityDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginUserHandler(UserManager<User> userManager, IAppIdentityDbContext context, ITokenService tokenService, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<LoginUserResponse>.Failure("Неверный email или пароль");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
            return Result<LoginUserResponse>.Failure("Неверный email или пароль");

        string token = await _tokenService.GenerateTokenAsync(user);
        string refreshToken = _tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            }
        );

        await _context.SaveChangesAsync(cancellationToken);

        return Result<LoginUserResponse>.Success(new LoginUserResponse(token));
    }
}