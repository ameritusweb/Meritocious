using Meritocious.Blazor.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace Meritocious.Blazor.Services.Api;

public interface IAuthApiService
{
    Task<bool> IsAuthenticatedAsync();
    Task<AuthenticationResult> GoogleSignInAsync(string idToken);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task LinkGoogleAccountAsync(string idToken);
    Task UnlinkGoogleAccountAsync();
    Task SignOutAsync();
    event Func<Task> OnAuthenticationStateChanged;
}

public class AuthApiService : IAuthApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<AuthApiService> _logger;
    private readonly ITokenManager _tokenManager;
    private readonly NavigationManager _navigationManager;

    public event Func<Task> OnAuthenticationStateChanged;

    public AuthApiService(
        ApiClient apiClient,
        ILogger<AuthApiService> logger,
        ITokenManager tokenManager,
        NavigationManager navigationManager)
    {
        _apiClient = apiClient;
        _logger = logger;
        _tokenManager = tokenManager;
        _navigationManager = navigationManager;

        _tokenManager.OnTokensChanged += HandleTokensChanged;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await _tokenManager.IsAuthenticatedAsync();
    }

    public async Task<AuthenticationResult> GoogleSignInAsync(string idToken)
    {
        try
        {
            var result = await _apiClient.PostAsync<AuthenticationResult>(
                "api/auth/google",
                new { IdToken = idToken });

            await _tokenManager.SetTokensAsync(result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google sign-in");
            throw;
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Use a temporary HttpClient without auth headers for refresh
            var result = await _apiClient.PostAsync<AuthenticationResult>(
                "api/auth/refresh",
                new { RefreshToken = refreshToken });

            await _tokenManager.SetTokensAsync(result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            await _tokenManager.ClearTokensAsync();
            throw;
        }
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        try
        {
            await _apiClient.PostAsync<Unit>(
                "api/auth/revoke",
                new { RefreshToken = refreshToken });

            await _tokenManager.ClearTokensAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            // Still clear tokens locally even if remote revocation fails
            await _tokenManager.ClearTokensAsync();
            throw;
        }
    }

    public async Task LinkGoogleAccountAsync(string idToken)
    {
        try
        {
            await _apiClient.PostAsync<Unit>(
                "api/auth/google/link",
                new { IdToken = idToken });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking Google account");
            throw;
        }
    }

    public async Task UnlinkGoogleAccountAsync()
    {
        try
        {
            await _apiClient.PostAsync<Unit>("api/auth/google/unlink", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking Google account");
            throw;
        }
    }

    public async Task SignOutAsync()
    {
        try
        {
            var refreshToken = await _tokenManager.GetRefreshTokenAsync();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await RevokeTokenAsync(refreshToken);
            }
            
            await _tokenManager.ClearTokensAsync();
            _navigationManager.NavigateTo("/", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign out");
            // Ensure tokens are cleared even if remote revocation fails
            await _tokenManager.ClearTokensAsync();
            throw;
        }
    }

    private async Task HandleTokensChanged()
    {
        if (OnAuthenticationStateChanged != null)
        {
            await OnAuthenticationStateChanged.Invoke();
        }
    }

    public void Dispose()
    {
        _tokenManager.OnTokensChanged -= HandleTokensChanged;
    }
}

public class AuthenticationResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}