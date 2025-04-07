using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using System.Security.Claims;

namespace Meritocious.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthenticationService authService;
        private readonly ILogger<AuthController> logger;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AuthController(
            IAuthenticationService authService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized(new { Error = "Invalid email or password" });

            var result = await signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!result.Succeeded)
                return Unauthorized(new { Error = "Invalid email or password" });

            // Check if Google account is linked
            var logins = await userManager.GetLoginsAsync(user);
            if (!logins.Any(l => l.LoginProvider == "Google"))
            {
                return Ok(new { RequiresGoogleLink = true });
            }

            var authResult = await authService.GenerateAuthTokensAsync(user);
            return Ok(new LoginResponse 
            { 
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken,
                ExpiresAt = authResult.ExpiresAt,
                User = user.ToDto(),
                RequiresTwoFactor = await authService.RequiresTwoFactorAsync(user.Id)
            });
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResult>> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            var result = await authService.AuthenticateGoogleUserAsync(request.IdToken);
            return HandleResult(result);
        }

        [Authorize(Policy = "NoGoogleRequired")]
        [HttpPost("google/link")]
        public async Task<ActionResult<AuthenticationResult>> LinkGoogleAccount([FromBody] GoogleSignInRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await authService.LinkGoogleAccountAsync(userId, request.IdToken);
            if (!result.Success)
                return HandleResult(result);

            var user = await userManager.FindByIdAsync(userId);
            var authResult = await authService.GenerateAuthTokensAsync(user);
            return Ok(new LoginResponse
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken,
                ExpiresAt = authResult.ExpiresAt,
                User = user.ToDto(),
                RequiresTwoFactor = await authService.RequiresTwoFactorAsync(userId)
            });
        }

        [Authorize]
        [HttpPost("google/unlink")]
        public async Task<ActionResult> UnlinkGoogleAccount()
        {
            var userId = GetCurrentUserId();
            var result = await authService.UnlinkGoogleAccountAsync(userId);
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticationResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        [HttpPost("revoke")]
        public async Task<ActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RevokeTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Ulid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }

            return userId.ToString();
        }
    }

    public class GoogleSignInRequest
    {
        public string IdToken { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserProfileDto User { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool RequiresGoogleLink { get; set; }
    }
}