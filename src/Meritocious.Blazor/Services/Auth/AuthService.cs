using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Meritocious.Blazor.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                if (!response.IsSuccessStatusCode || result == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Error = "Login failed"
                    };
                }

                await _localStorage.SetItemAsync("authToken", result.Token);
                await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

                ((ApiAuthenticationStateProvider)_authStateProvider)
                    .MarkUserAsAuthenticated(result.Token);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return new LoginResult
                {
                    Success = false,
                    Error = "An error occurred during login"
                };
            }
        }

        public async Task<LoginResult> GoogleLoginAsync(string idToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/google",
                    new { IdToken = idToken });

                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                if (!response.IsSuccessStatusCode || result == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Error = "Google login failed"
                    };
                }

                await _localStorage.SetItemAsync("authToken", result.Token);
                await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

                ((ApiAuthenticationStateProvider)_authStateProvider)
                    .MarkUserAsAuthenticated(result.Token);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google login failed");
                return new LoginResult
                {
                    Success = false,
                    Error = "An error occurred during Google login"
                };
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _httpClient.PostAsJsonAsync("api/auth/revoke",
                        new { RefreshToken = refreshToken });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
            }

            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");

            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                var response = await _httpClient.PostAsJsonAsync("api/auth/refresh",
                    new { RefreshToken = refreshToken });

                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                if (!response.IsSuccessStatusCode || result == null)
                {
                    await LogoutAsync();
                    return false;
                }

                await _localStorage.SetItemAsync("authToken", result.Token);
                await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

                ((ApiAuthenticationStateProvider)_authStateProvider)
                    .MarkUserAsAuthenticated(result.Token);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                await LogoutAsync();
                return false;
            }
        }
    }
}
