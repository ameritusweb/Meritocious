using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Interfaces;

namespace Meritocious.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RemixController : ApiControllerBase
{
    private readonly IRemixService _remixService;

    public RemixController(IRemixService remixService)
    {
        _remixService = remixService;
    }

    /// <summary>
    /// Creates a new remix
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RemixDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRemix([FromBody] CreateRemixRequest request)
    {
        request.AuthorId = UserId;
        var remix = await _remixService.CreateRemixAsync(request);
        return CreatedAtAction(nameof(GetRemix), new { id = remix.Id }, remix);
    }

    /// <summary>
    /// Gets a remix by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RemixDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRemix(Guid id)
    {
        var remix = await _remixService.GetRemixByIdAsync(id);
        return Ok(remix);
    }

    /// <summary>
    /// Updates an existing remix
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RemixDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRemix(Guid id, [FromBody] UpdateRemixRequest request)
    {
        var remix = await _remixService.GetRemixByIdAsync(id);
        if (remix.AuthorId != UserId)
        {
            return Forbid();
        }

        var updated = await _remixService.UpdateRemixAsync(id, request);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a remix
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRemix(Guid id)
    {
        var success = await _remixService.DeleteRemixAsync(id, UserId);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Publishes a draft remix
    /// </summary>
    [HttpPost("{id}/publish")]
    [ProducesResponseType(typeof(RemixDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishRemix(Guid id)
    {
        var published = await _remixService.PublishRemixAsync(id, UserId);
        return Ok(published);
    }

    /// <summary>
    /// Adds a source to a remix
    /// </summary>
    [HttpPost("{id}/sources")]
    [ProducesResponseType(typeof(RemixSourceDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddSource(Guid id, [FromBody] AddSourceRequest request)
    {
        var source = await _remixService.AddSourceAsync(id, request);
        return CreatedAtAction(nameof(GetRemix), new { id }, source);
    }

    /// <summary>
    /// Removes a source from a remix
    /// </summary>
    [HttpDelete("{id}/sources/{sourceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveSource(Guid id, Guid sourceId)
    {
        var success = await _remixService.RemoveSourceAsync(id, sourceId);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Updates source relationships or order
    /// </summary>
    [HttpPut("{id}/sources")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateSources(Guid id, [FromBody] UpdateSourcesRequest request)
    {
        if (request.OrderUpdates != null)
        {
            await _remixService.UpdateSourceOrderAsync(id, request.OrderUpdates);
        }

        if (request.RelationshipUpdate != null)
        {
            await _remixService.UpdateSourceRelationshipAsync(
                id, 
                request.RelationshipUpdate.SourceId, 
                request.RelationshipUpdate.Relationship);
        }

        return NoContent();
    }

    /// <summary>
    /// Adds a quote to a source
    /// </summary>
    [HttpPost("{id}/sources/{sourceId}/quotes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddQuote(Guid id, Guid sourceId, [FromBody] AddQuoteRequest request)
    {
        await _remixService.AddQuoteToSourceAsync(sourceId, request);
        return NoContent();
    }

    /// <summary>
    /// Gets remixes by the current user
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<RemixDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRemixes([FromQuery] RemixFilter filter)
    {
        var remixes = await _remixService.GetUserRemixesAsync(UserId, filter);
        return Ok(remixes);
    }

    /// <summary>
    /// Gets related remixes
    /// </summary>
    [HttpGet("{id}/related")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RemixDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRelatedRemixes(Guid id, [FromQuery] int limit = 5)
    {
        var related = await _remixService.GetRelatedRemixesAsync(id, limit);
        return Ok(related);
    }

    /// <summary>
    /// Gets trending remixes
    /// </summary>
    [HttpGet("trending")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RemixDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrendingRemixes([FromQuery] int limit = 10)
    {
        var trending = await _remixService.GetTrendingRemixesAsync(limit);
        return Ok(trending);
    }

    /// <summary>
    /// Gets analytics for a remix
    /// </summary>
    [HttpGet("{id}/analytics")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RemixAnalytics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnalytics(Guid id)
    {
        var analytics = await _remixService.GetRemixAnalyticsAsync(id);
        return Ok(analytics);
    }

    /// <summary>
    /// Searches remixes
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RemixDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchRemixes([FromQuery] RemixSearchRequest request)
    {
        var results = await _remixService.SearchRemixesAsync(request);
        return Ok(results);
    }

    /// <summary>
    /// Generates AI insights for a remix
    /// </summary>
    [HttpPost("{id}/insights")]
    [ProducesResponseType(typeof(IEnumerable<RemixNoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateInsights(Guid id)
    {
        var insights = await _remixService.GenerateInsightsAsync(id);
        return Ok(insights);
    }

    /// <summary>
    /// Gets AI suggestions for a remix
    /// </summary>
    [HttpGet("{id}/suggestions")]
    [ProducesResponseType(typeof(IEnumerable<RemixNoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuggestions(Guid id)
    {
        var suggestions = await _remixService.GetSuggestionsAsync(id);
        return Ok(suggestions);
    }

    /// <summary>
    /// Gets the synthesis score for a remix
    /// </summary>
    [HttpGet("{id}/score")]
    [ProducesResponseType(typeof(RemixScoreResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScore(Guid id)
    {
        var score = await _remixService.CalculateRemixScoreAsync(id);
        return Ok(score);
    }
}

public class UpdateSourcesRequest
{
    public IEnumerable<SourceOrderUpdate> OrderUpdates { get; set; }
    public RelationshipUpdate RelationshipUpdate { get; set; }
}

public class RelationshipUpdate
{
    public Guid SourceId { get; set; }
    public string Relationship { get; set; }
}