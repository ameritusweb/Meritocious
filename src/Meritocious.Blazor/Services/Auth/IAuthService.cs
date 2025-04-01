namespace Meritocious.Blazor.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<LoginResult> GoogleLoginAsync(string idToken);
        Task LogoutAsync();
        Task<bool> RefreshTokenAsync();
    }
}
