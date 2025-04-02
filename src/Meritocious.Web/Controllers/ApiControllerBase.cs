using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Results;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated");
        return userId;
    }

    protected ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsFailure)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok();
    }
}