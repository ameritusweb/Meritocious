using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Features.Substacks.Queries;
using Meritocious.Core.Features.Substacks.Commands;
using Meritocious.Core.Features.Substacks.Models;
using Meritocious.Core.Features.Substacks.Services;

namespace Meritocious.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubstackController : ApiControllerBase
{
    private readonly IMediator mediator;

    public SubstackController(IMediator mediator) : base()
    {
        this.mediator = mediator;
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<SubstackDto>>> GetTrendingSubstacks(
        [FromQuery] int limit = 10,
        [FromQuery] int skip = 0)
    {
        var result = await mediator.Send(new GetTrendingSubstacksQuery(limit, skip));
        return HandleResult(result);
    }

    [HttpGet("recommended")]
    public async Task<ActionResult<List<SubstackDto>>> GetRecommendedSubstacks(
        [FromQuery] int limit = 10)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetRecommendedSubstacksQuery(userId, limit));
        return Ok(result);
    }

    [HttpGet("{id}/similar")]
    public async Task<ActionResult<List<SubstackDto>>> GetSimilarSubstacks(
        string id,
        [FromQuery] int limit = 5)
    {
        var result = await mediator.Send(new GetSimilarSubstacksQuery(id, limit));
        return Ok(result);
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<SubstackDto>>> GetFollowedSubstacks(
        [FromQuery] int limit = 20,
        [FromQuery] int skip = 0)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetFollowedSubstacksQuery(userId, limit, skip));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubstackDto>> GetSubstack(string id)
    {
        var result = await mediator.Send(new GetSubstackQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SubstackDto>> CreateSubstack(CreateSubstackCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubstackDto>> UpdateSubstack(
        string id,
        UpdateSubstackCommand command)
    {
        if (id != command.SubstackId)
        {
            return BadRequest("ID mismatch");
        }

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/follow")]
    public async Task<ActionResult<bool>> FollowSubstack(string id)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new FollowSubstackCommand(userId, id));
        return Ok(result);
    }

    [HttpDelete("{id}/follow")]
    public async Task<ActionResult<bool>> UnfollowSubstack(string id)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new UnfollowSubstackCommand(userId, id));
        return Ok(result);
    }

    [HttpGet("{id}/metrics")]
    public async Task<ActionResult<SubstackMetricsDto>> GetSubstackMetrics(string id)
    {
        var result = await mediator.Send(new GetSubstackMetricsQuery(id));
        return Ok(result);
    }

    [HttpPost("import")]
    public async Task<ActionResult<string>> ImportSubstackPost(SubstackPostImportRequest request)
    {
        var command = new ImportSubstackPostCommand
        {
            PostUrl = request.PostUrl,
            SubstackName = request.SubstackName,
            UserId = GetUserId(),
            ImportAsRemix = request.ImportAsRemix,
            RemixNotes = request.RemixNotes
        };
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("validate")]
    public async Task<ActionResult<bool>> ValidateSubstackUrl([FromQuery] string url)
    {
        var substackService = HttpContext.RequestServices.GetRequiredService<ISubstackFeedService>();
        var isValid = await substackService.ValidateSubstackUrlAsync(url);
        return Ok(isValid);
    }

    [HttpGet("posts")]
    public async Task<ActionResult<SubstackFeedResponse>> GetSubstackFeed([FromQuery] string url)
    {
        try
        {
            var substackService = HttpContext.RequestServices.GetRequiredService<ISubstackFeedService>();
            var feed = await substackService.GetPublicationFeedAsync(url);
            return Ok(feed);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Failed to fetch Substack feed" });
        }
    }
}