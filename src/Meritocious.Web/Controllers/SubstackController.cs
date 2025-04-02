using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Common.DTOs.Content;
using MediatR;

namespace Meritocious.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubstackController : ApiControllerBase
{
    public SubstackController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<SubstackDto>>> GetTrendingSubstacks(
        [FromQuery] string period = "day",
        [FromQuery] int count = 10)
    {
        var query = new GetTrendingSubstacksQuery { Period = period, Count = count };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("recommended")]
    public async Task<ActionResult<List<SubstackDto>>> GetRecommendedSubstacks(
        [FromQuery] int count = 10)
    {
        var query = new GetRecommendedSubstacksQuery { Count = count };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id}/similar")]
    public async Task<ActionResult<List<SubstackDto>>> GetSimilarSubstacks(
        Guid id,
        [FromQuery] int count = 10)
    {
        var query = new GetSimilarSubstacksQuery { SubstackId = id, Count = count };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<SubstackDto>>> GetFollowedSubstacks()
    {
        var userId = GetUserId();
        var query = new GetFollowedSubstacksQuery { UserId = userId };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<SubstackDto>> GetSubstack(string slug)
    {
        var query = new GetSubstackQuery { Slug = slug };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<SubstackDto>> CreateSubstack(CreateSubstackCommand command)
    {
        command.UserId = GetUserId();
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{slug}")]
    public async Task<ActionResult<SubstackDto>> UpdateSubstack(
        string slug,
        UpdateSubstackCommand command)
    {
        if (slug != command.Slug)
            return BadRequest("Slug mismatch");

        command.UserId = GetUserId();
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{slug}/follow")]
    public async Task<ActionResult<SubstackDto>> FollowSubstack(string slug)
    {
        var command = new FollowSubstackCommand
        {
            UserId = GetUserId(),
            Slug = slug
        };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{slug}/follow")]
    public async Task<ActionResult> UnfollowSubstack(string slug)
    {
        var command = new UnfollowSubstackCommand
        {
            UserId = GetUserId(),
            Slug = slug
        };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{slug}/metrics")]
    public async Task<ActionResult<SubstackMetricsDto>> GetSubstackMetrics(string slug)
    {
        var query = new GetSubstackMetricsQuery { Slug = slug };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}