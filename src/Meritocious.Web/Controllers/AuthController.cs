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

        public AuthController(
            IAuthenticationService authService,
            ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.logger = logger;
        }

        [HttpPost("google")]
        public async Task<ActionResult<AuthenticationResult>> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            var result = await authService.AuthenticateGoogleUserAsync(request.IdToken);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("google/link")]
        public async Task<ActionResult> LinkGoogleAccount([FromBody] GoogleSignInRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await authService.LinkGoogleAccountAsync(userId, request.IdToken);
            return HandleResult(result);
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

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }

            return userId;
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
}