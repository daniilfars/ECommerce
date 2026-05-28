using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Interfaces;
using Modules.Identity.Domain;
using Shared.Domain;

namespace Modules.Identity.Application.Commands.Refresh;

public class RefreshUserHandler : IRequestHandler<RefreshUserCommand, Result<RefreshUserResponse>>
{
    private readonly IAppIdentityDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshUserHandler(IAppIdentityDbContext context, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<RefreshUserResponse>> Handle(RefreshUserCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<RefreshUserResponse>.Failure("Refresh-токен отсутствует");

        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == refreshToken, cancellationToken);

        if (user is not null)
        {
            if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                user.ClearRefreshToken();
                await _context.SaveChangesAsync(cancellationToken);
                return Result<RefreshUserResponse>.Failure("Refresh-токен истёк");
            }

            return await RotateTokens(user, cancellationToken);
        }

        user = await _context.Users.FirstOrDefaultAsync(
            u => u.PreviousRefreshToken == refreshToken, cancellationToken);

        if (user is not null) // Если true, то это replay-атака: Инвалидируем все сессии
        {
            user.ClearRefreshToken();
            await _context.SaveChangesAsync(cancellationToken);

            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken"); // Удаляем куку у злоумышленника

            return Result<RefreshUserResponse>.Failure("Обнаружено повторное использование refresh-токена. Все сессии сброшены.");
        }

        return Result<RefreshUserResponse>.Failure("Недействительный refresh-токен");
    }

    private async Task<Result<RefreshUserResponse>> RotateTokens(User user, CancellationToken cancellationToken)
    {
        var newAccessToken = await _tokenService.GenerateTokenAsync(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));

        _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", newRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            }
        );

        await _context.SaveChangesAsync(cancellationToken);

        return Result<RefreshUserResponse>.Success(new RefreshUserResponse(newAccessToken));
    }
}