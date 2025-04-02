using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AspNetCoreRateLimit;

namespace Meritocious.Web.Filters;

public class RateLimitExceededFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if request was blocked by rate limiting
        var feature = context.HttpContext.Features.Get<IClientRateLimitFeature>();
        if (feature?.Limit != null)
        {
            var quota = feature.Limit;
            var remaining = quota.Remaining;
            
            // Add rate limit headers
            context.HttpContext.Response.Headers["X-RateLimit-Limit"] = quota.Limit.ToString();
            context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            context.HttpContext.Response.Headers["X-RateLimit-Reset"] = quota.Reset.ToUnixTimeSeconds().ToString();

            if (remaining <= 0)
            {
                var problem = new ProblemDetails
                {
                    Title = "Too Many Requests",
                    Detail = "API rate limit has been exceeded",
                    Status = StatusCodes.Status429TooManyRequests,
                    Type = "https://tools.ietf.org/html/rfc6585#section-4"
                };

                context.Result = new ObjectResult(problem)
                {
                    StatusCode = StatusCodes.Status429TooManyRequests
                };
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Method intentionally left empty
    }
}