using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Identity.Application.Interfaces;
using Shared.Domain;

namespace Identity.Application.Commands.Logout;

public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result>
{
    private readonly IAppIdentityDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogoutUserHandler(IAppIdentityDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Result.Success();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        if (user is not null)
        {
            user.ClearRefreshToken();
            await _context.SaveChangesAsync(cancellationToken);
        }

        _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");

        return Result.Success();
    }
}
