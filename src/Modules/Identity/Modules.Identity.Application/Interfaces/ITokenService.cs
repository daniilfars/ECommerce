using Modules.Identity.Domain;
using System.Security.Claims;

namespace Modules.Identity.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
