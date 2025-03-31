using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly GoogleAuthSettings _googleSettings;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserRepository userRepository,
            ITokenService tokenService,
            IOptions<GoogleAuthSettings> googleSettings,
            ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _googleSettings = googleSettings.Value;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResult>> AuthenticateGoogleUserAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleSettings.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Check if user exists with this Google ID
                var externalLogin = await _userRepository.GetExternalLoginAsync("Google", payload.Subject);

                if (externalLogin != null)
                {
                    // Update existing login info
                    externalLogin.UpdateLoginInfo(
                        payload.Name,
                        payload.Picture,
                        _tokenService.GenerateRefreshToken(),
                        DateTime.UtcNow.AddDays(30));

                    await _userRepository.UpdateExternalLoginAsync(externalLogin);

                    return Result.Success(new AuthenticationResult
                    {
                        AccessToken = _tokenService.GenerateAccessToken(externalLogin.User),
                        RefreshToken = externalLogin.RefreshToken,
                        ExpiresAt = _tokenService.GetAccessTokenExpiration(),
                        User = externalLogin.User.ToDto(),
                        IsNewUser = false
                    });
                }

                // Check if user exists with this email
                var user = await _userRepository.GetByEmailAsync(payload.Email);
                bool isNewUser = false;

                if (user == null)
                {
                    // Create new user
                    user = User.Create(
                        payload.Email.Split('@')[0], // Use email prefix as username
                        payload.Email,
                        Guid.NewGuid().ToString()); // Random password for external accounts

                    await _userRepository.AddAsync(user);
                    isNewUser = true;
                }

                // Create external login
                var newExternalLogin = ExternalLogin.Create(
                    user,
                    "Google",
                    payload.Subject,
                    payload.Email,
                    payload.Name,
                    payload.Picture,
                    _tokenService.GenerateRefreshToken(),
                    DateTime.UtcNow.AddDays(30));

                await _userRepository.AddExternalLoginAsync(newExternalLogin);

                return Result.Success(new AuthenticationResult
                {
                    AccessToken = _tokenService.GenerateAccessToken(user),
                    RefreshToken = newExternalLogin.RefreshToken,
                    ExpiresAt = _tokenService.GetAccessTokenExpiration(),
                    User = user.ToDto(),
                    IsNewUser = isNewUser
                });
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google ID token");
                return Result.Failure<AuthenticationResult>("Invalid Google authentication token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating Google user");
                return Result.Failure<AuthenticationResult>("An error occurred during authentication");
            }
        }

        public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var externalLogin = await _userRepository.GetExternalLoginByRefreshTokenAsync(refreshToken);

                if (externalLogin == null ||
                    externalLogin.RefreshTokenExpiresAt <= DateTime.UtcNow)
                {
                    return Result.Failure<AuthenticationResult>("Invalid or expired refresh token");
                }

                // Generate new tokens
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                externalLogin.UpdateLoginInfo(
                    externalLogin.Name,
                    externalLogin.PictureUrl,
                    newRefreshToken,
                    DateTime.UtcNow.AddDays(30));

                await _userRepository.UpdateExternalLoginAsync(externalLogin);

                return Result.Success(new AuthenticationResult
                {
                    AccessToken = _tokenService.GenerateAccessToken(externalLogin.User),
                    RefreshToken = newRefreshToken,
                    ExpiresAt = _tokenService.GetAccessTokenExpiration(),
                    User = externalLogin.User.ToDto(),
                    IsNewUser = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Result.Failure<AuthenticationResult>("An error occurred while refreshing the token");
            }
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var externalLogin = await _userRepository.GetExternalLoginByRefreshTokenAsync(refreshToken);

                if (externalLogin == null)
                {
                    return Result.Success(); // Token already revoked or doesn't exist
                }

                externalLogin.UpdateLoginInfo(
                    externalLogin.Name,
                    externalLogin.PictureUrl,
                    null,
                    null);

                await _userRepository.UpdateExternalLoginAsync(externalLogin);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return Result.Failure("An error occurred while revoking the token");
            }
        }

        public async Task<Result> LinkGoogleAccountAsync(Guid userId, string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleSettings.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Check if this Google account is already linked to another user
                var existingLogin = await _userRepository.GetExternalLoginAsync("Google", payload.Subject);
                if (existingLogin != null)
                {
                    return Result.Failure("This Google account is already linked to another user");
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                // Create external login
                // Create external login
                var newExternalLogin = ExternalLogin.Create(
                    user,
                    "Google",
                    payload.Subject,
                    payload.Email,
                    payload.Name,
                    payload.Picture,
                    _tokenService.GenerateRefreshToken(),
                    DateTime.UtcNow.AddDays(30));

                await _userRepository.AddExternalLoginAsync(newExternalLogin);
                return Result.Success();
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google ID token");
                return Result.Failure("Invalid Google authentication token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking Google account");
                return Result.Failure("An error occurred while linking the Google account");
            }
        }

        public async Task<Result> UnlinkGoogleAccountAsync(Guid userId)
        {
            try
            {
                var externalLogin = await _userRepository.GetExternalLoginByUserIdAsync(userId, "Google");
                if (externalLogin == null)
                {
                    return Result.Failure("No Google account linked to this user");
                }

                await _userRepository.RemoveExternalLoginAsync(externalLogin);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking Google account");
                return Result.Failure("An error occurred while unlinking the Google account");
            }
        }
    }

    public class GoogleAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}