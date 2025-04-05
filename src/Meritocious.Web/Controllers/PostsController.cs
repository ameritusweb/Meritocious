using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Commands;
using Meritocious.Core.Features.Posts.Commands;
using Meritocious.Core.Features.Posts.Queries;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Comments.Queries;
using Meritocious.Core.Features.Comments.Commands;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IMediator mediator, ILogger<PostsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetTopPosts([FromQuery] int count = 10, [FromQuery] string sortBy = "merit")
    {
        var query = new GetTopPostsQuery { Count = count, SortBy = sortBy };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(Guid id)
    {
        var query = new GetPostQuery { PostId = id };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost(CreatePostCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPost), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PostDto>> UpdatePost(Guid id, UpdatePostCommand command)
    {
        if (id != command.PostId)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id}/fork")]
    public async Task<ActionResult<PostDto>> ForkPost(Guid id, ForkPostCommand command)
    {
        if (id != command.OriginalPostId)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return NotFound(result.Error);

        return CreatedAtAction(nameof(GetPost), new { id = result.Value.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(Guid id)
    {
        await _mediator.Send(new DeletePostCommand { PostId = id });
        return NoContent();
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<List<CommentDto>>> GetPostComments(
        Guid id,
        [FromQuery] string sortBy = "merit",
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        var query = new GetPostCommentsQuery
        {
            PostId = id,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(Guid id, AddCommentCommand command)
    {
        if (id != command.PostId)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<PostSummaryDto>>> GetTrendingPosts(
        [FromQuery] string timeFrame = "day",
        [FromQuery] string? category = null,
        [FromQuery] int limit = 10,
        [FromQuery] decimal minMeritScore = 0.0m)
    {
        try
        {
            var query = new GetTrendingPostsQuery(timeFrame, category, limit, minMeritScore);
            var posts = await _mediator.Send(query);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending posts");
            return StatusCode(500, "Error retrieving trending posts");
        }
    }

    [HttpGet("{id}/history")]
    public async Task<ActionResult<List<PostVersionDto>>> GetPostHistory(
        Guid id,
        [FromQuery] int? startVersion = null,
        [FromQuery] int? endVersion = null,
        [FromQuery] bool includeContent = true)
    {
        try
        {
            var query = new GetPostHistoryQuery(id, startVersion, endVersion, includeContent);
            var history = await _mediator.Send(query);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post history for post {PostId}", id);
            return StatusCode(500, "Error retrieving post history");
        }
    }

    [HttpGet("{id}/versions/{versionNumber}")]
    public async Task<ActionResult<PostVersionDto>> GetPostVersion(Guid id, int versionNumber)
    {
        try
        {
            var query = new GetPostHistoryQuery(id, versionNumber, versionNumber);
            var versions = await _mediator.Send(query);
            var version = versions.FirstOrDefault();
                
            if (version == null)
            {
                return NotFound($"Version {versionNumber} not found for post {id}");
            }

            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting version {Version} for post {PostId}", versionNumber, id);
            return StatusCode(500, "Error retrieving post version");
        }
    }

    [HttpGet("{id}/versions/compare")]
    public async Task<ActionResult<PostVersionComparisonDto>> CompareVersions(
        Guid id,
        [FromQuery] int version1,
        [FromQuery] int version2)
    {
        try
        {
            // Get both versions
            var query = new GetPostHistoryQuery(id, 
                Math.Min(version1, version2), 
                Math.Max(version1, version2));
                
            var versions = await _mediator.Send(query);
                
            var oldVersion = versions.FirstOrDefault(v => v.VersionNumber == Math.Min(version1, version2));
            var newVersion = versions.FirstOrDefault(v => v.VersionNumber == Math.Max(version1, version2));

            if (oldVersion == null || newVersion == null)
            {
                return NotFound("One or both versions not found");
            }

            // Compare versions
            var comparison = new PostVersionComparisonDto
            {
                OldVersion = oldVersion,
                NewVersion = newVersion,
                Differences = ComputeDifferences(oldVersion, newVersion)
            };

            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing versions {V1} and {V2} for post {PostId}", 
                version1, version2, id);
            return StatusCode(500, "Error comparing post versions");
        }
    }

    private static List<PostVersionComparisonDto.VersionDiffDto> ComputeDifferences(
        PostVersionDto oldVersion,
        PostVersionDto newVersion)
    {
        var diffs = new List<PostVersionComparisonDto.VersionDiffDto>();

        // Compare title
        if (oldVersion.Title != newVersion.Title)
        {
            diffs.Add(new PostVersionComparisonDto.VersionDiffDto
            {
                Field = "Title",
                OldValue = oldVersion.Title,
                NewValue = newVersion.Title,
                DiffType = "Modified"
            });
        }

        // Compare content using diff metrics
        if (oldVersion.Content != newVersion.Content)
        {
            diffs.Add(new PostVersionComparisonDto.VersionDiffDto
            {
                Field = "Content",
                OldValue = oldVersion.Content,
                NewValue = newVersion.Content,
                DiffType = "Modified",
                Metadata = new Dictionary<string, object>
                {
                    { "AddedLines", newVersion.AddedLines },
                    { "RemovedLines", newVersion.RemovedLines },
                    { "ModifiedLines", newVersion.ModifiedLines }
                }
            });
        }

        // Compare tags
        var removedTags = oldVersion.Tags.Except(newVersion.Tags).ToList();
        var addedTags = newVersion.Tags.Except(oldVersion.Tags).ToList();

        if (removedTags.Any() || addedTags.Any())
        {
            diffs.Add(new PostVersionComparisonDto.VersionDiffDto
            {
                Field = "Tags",
                OldValue = string.Join(", ", oldVersion.Tags),
                NewValue = string.Join(", ", newVersion.Tags),
                DiffType = removedTags.Any() && addedTags.Any() ? "Modified" :
                          addedTags.Any() ? "Added" : "Removed",
                Metadata = new Dictionary<string, object>
                {
                    { "AddedTags", addedTags },
                    { "RemovedTags", removedTags }
                }
            });
        }

        return diffs;
    }
}