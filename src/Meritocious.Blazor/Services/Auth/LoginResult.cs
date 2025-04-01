using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Blazor.Services.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public UserProfileDto? User { get; set; }
    }
}
