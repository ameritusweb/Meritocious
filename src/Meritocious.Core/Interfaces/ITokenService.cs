using System.Security.Claims;

namespace Meritocious.Core.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessToken(Entities.User user);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiration();
    Task<ClaimsPrincipal> ValidateToken(string token);
}