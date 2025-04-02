using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Meritocious.Blazor.Services.Auth;

public interface ITokenManager
{
    Task<string> GetAccessTokenAsync();
    Task SetTokensAsync(AuthenticationResult result);
    Task ClearTokensAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<DateTime?> GetTokenExpirationAsync();
    Task<string> GetRefreshTokenAsync();
    event Func<Task> OnTokensChanged;
}

public class TokenManager : ITokenManager
{
    private const string ACCESS_TOKEN_KEY = "access_token";
    private const string REFRESH_TOKEN_KEY = "refresh_token";
    private const string TOKEN_EXPIRATION_KEY = "token_expiration";

    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<TokenManager> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public event Func<Task> OnTokensChanged;

    public TokenManager(
        ILocalStorageService localStorage,
        ILogger<TokenManager> logger,
        IHttpClientFactory httpClientFactory,
        AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _authStateProvider = authStateProvider;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            await _semaphore.WaitAsync();

            var token = await _localStorage.GetItemAsync<string>(ACCESS_TOKEN_KEY);
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var expiration = await _localStorage.GetItemAsync<DateTime>(TOKEN_EXPIRATION_KEY);
            if (expiration <= DateTime.UtcNow.AddMinutes(5)) // Buffer time for renewal
            {
                _logger.LogInformation("Access token expired or expiring soon, attempting refresh");
                return await RefreshTokensAsync();
            }

            return token;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SetTokensAsync(AuthenticationResult result)
    {
        try
        {
            await _semaphore.WaitAsync();

            await _localStorage.SetItemAsync(ACCESS_TOKEN_KEY, result.AccessToken);
            await _localStorage.SetItemAsync(REFRESH_TOKEN_KEY, result.RefreshToken);
            await _localStorage.SetItemAsync(TOKEN_EXPIRATION_KEY, result.ExpiresAt);

            ConfigureHttpClient(result.AccessToken);
            await NotifyTokenChanged();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearTokensAsync()
    {
        try
        {
            await _semaphore.WaitAsync();

            await _localStorage.RemoveItemAsync(ACCESS_TOKEN_KEY);
            await _localStorage.RemoveItemAsync(REFRESH_TOKEN_KEY);
            await _localStorage.RemoveItemAsync(TOKEN_EXPIRATION_KEY);

            ConfigureHttpClient(null);
            await NotifyTokenChanged();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetAccessTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<DateTime?> GetTokenExpirationAsync()
    {
        return await _localStorage.GetItemAsync<DateTime?>(TOKEN_EXPIRATION_KEY);
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(REFRESH_TOKEN_KEY);
    }

    private async Task<string> RefreshTokensAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("No refresh token available");
                await ClearTokensAsync();
                return null;
            }

            // Create a temporary HttpClient without auth headers for the refresh request
            var http = _httpClientFactory.CreateClient();
            var response = await http.PostAsJsonAsync("api/auth/refresh", new { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Token refresh failed with status code {StatusCode}", response.StatusCode);
                await ClearTokensAsync();
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
            await SetTokensAsync(result);
            return result.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing tokens");
            await ClearTokensAsync();
            return null;
        }
    }

    private void ConfigureHttpClient(string token)
    {
        var http = _httpClientFactory.CreateClient("API");
        if (!string.IsNullOrEmpty(token))
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            http.DefaultRequestHeaders.Authorization = null;
        }
    }

    private async Task NotifyTokenChanged()
    {
        if (OnTokensChanged != null)
        {
            try
            {
                await OnTokensChanged.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in token change notification");
            }
        }

        // Force auth state refresh
        if (_authStateProvider is ApiAuthenticationStateProvider apiAuth)
        {
            await apiAuth.NotifyAuthenticationStateChanged();
        }
    }
}