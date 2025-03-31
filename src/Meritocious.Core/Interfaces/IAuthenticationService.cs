using Meritocious.Common.DTOs.Auth;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System.Security.Claims;

namespace Meritocious.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationResult>> AuthenticateGoogleUserAsync(string idToken);
        Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken);
        Task<Result> RevokeTokenAsync(string refreshToken);
        Task<Result> LinkGoogleAccountAsync(Guid userId, string idToken);
        Task<Result> UnlinkGoogleAccountAsync(Guid userId);
    }

    public class AuthenticationResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserProfileDto User { get; set; }
        public bool IsNewUser { get; set; }
    }

    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiration();
        ClaimsPrincipal ValidateToken(string token);
    }
}