using Identity.Domain;
using System.Security.Claims;

namespace Identity.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
