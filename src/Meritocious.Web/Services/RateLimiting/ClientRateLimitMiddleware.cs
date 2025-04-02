using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Meritocious.Web.Services.RateLimiting;

public class ClientRateLimitMiddleware : IClientResolveContributor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientRateLimitMiddleware(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> ResolveClientAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        // First check for API key in header
        var apiKey = httpContext?.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKey))
        {
            return Task.FromResult(apiKey);
        }

        // Then check for authenticated user
        var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            return Task.FromResult("authenticated-user");
        }

        // Finally, use IP address for anonymous users
        return Task.FromResult("anonymous");
    }
}