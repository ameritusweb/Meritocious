using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Blazor.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<LoginResult> GoogleLoginAsync(string idToken);
        Task<LoginResult> LinkGoogleAccountAsync(string idToken);
        Task<LoginResult> RegisterAsync(RegistrationRequest request);
        Task LogoutAsync();
        Task<bool> RefreshTokenAsync();
        Task<bool> RequiresTwoFactorAsync();
        Task<TwoFactorSetupResult> SetupTwoFactorAsync();
        Task<bool> ValidateTwoFactorCodeAsync(string code);
        Task<UserSettingsDto> GetUserSettingsAsync();
        Task DisableTwoFactorAsync();
        Task DeleteAccountAsync();
    }
}