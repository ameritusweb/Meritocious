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

        public async Task<LoginResult> RegisterAsync(RegistrationRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                if (!response.IsSuccessStatusCode || result == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Error = "Registration failed"
                    };
                }

                if (!result.RequiresGoogleLink)
                {
                    await _localStorage.SetItemAsync("authToken", result.AccessToken);
                    await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

                    ((ApiAuthenticationStateProvider)_authStateProvider)
                        .MarkUserAsAuthenticated(result.AccessToken);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return new LoginResult
                {
                    Success = false,
                    Error = "An error occurred during registration"
                };
            }
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

        public async Task<LoginResult> LinkGoogleAccountAsync(string idToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/google/link",
                    new { IdToken = idToken });

                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                if (!response.IsSuccessStatusCode || result == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Error = "Failed to link Google account"
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
                _logger.LogError(ex, "Google account linking failed");
                return new LoginResult
                {
                    Success = false,
                    Error = "An error occurred while linking Google account"
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

        public async Task<bool> RequiresTwoFactorAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/auth/requires-2fa");
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking 2FA requirement");
                return false;
            }
        }

        public async Task<TwoFactorSetupResult> SetupTwoFactorAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/auth/setup-2fa", null);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to setup 2FA");
                }
                return await response.Content.ReadFromJsonAsync<TwoFactorSetupResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up 2FA");
                throw;
            }
        }

        public async Task<bool> ValidateTwoFactorCodeAsync(string code)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/validate-2fa", new { Code = code });
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating 2FA code");
                return false;
            }
        }

        public async Task<UserSettingsDto> GetUserSettingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users/settings");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get user settings");
                }
                return await response.Content.ReadFromJsonAsync<UserSettingsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user settings");
                throw;
            }
        }

        public async Task DisableTwoFactorAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/auth/disable-2fa", null);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to disable 2FA");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling 2FA");
                throw;
            }
        }

        public async Task DeleteAccountAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/users/account");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to delete account");
                }
                await LogoutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account");
                throw;
            }
        }
    }
}
