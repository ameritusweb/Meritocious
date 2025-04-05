using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Discovery.Queries;
using Meritocious.Core.Results;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Core.Features.Tags.Queries;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITagService _tagService;

    public TagsController(IMediator mediator, ITagService tagService)
    {
        _mediator = mediator;
        _tagService = tagService;
    }

    [HttpGet("popular")]
    public async Task<ActionResult<List<Tag>>> GetPopularTags([FromQuery] int count = 10)
    {
        var tags = await _tagService.GetPopularTagsAsync(count);
        return Ok(tags);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Tag>>> SearchTags([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        var tags = await _tagService.SearchTagsAsync(searchTerm);
        return Ok(tags);
    }

    [HttpGet("{name}/posts")]
    public async Task<ActionResult<List<PostDto>>> GetPostsByTag(
        string name,
        [FromQuery] string sortBy = "merit",
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        var query = new GetPostsByTagQuery
        {
            TagName = name,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(CreateTagCommand command)
    {
        var tag = await _tagService.CreateTagAsync(command.Name, command.Description);
        return CreatedAtAction(nameof(GetTag), new { name = tag.Name }, tag);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<Tag>> GetTag(string name)
    {
        var tag = await _tagService.GetTagByNameAsync(name);
        if (tag == null)
            return NotFound();

        return Ok(tag);
    }

    [HttpPost("{name}/posts/{postId}")]
    public async Task<ActionResult> AddTagToPost(string name, Guid postId)
    {
        await _tagService.AddTagToPostAsync(postId, name);
        return NoContent();
    }

    [HttpDelete("{name}/posts/{postId}")]
    public async Task<ActionResult> RemoveTagFromPost(string name, Guid postId)
    {
        await _tagService.RemoveTagFromPostAsync(postId, name);
        return NoContent();
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<TrendingTopicDto>>> GetTrendingTopics(
        [FromQuery] int count = 10,
        [FromQuery] string timeFrame = "day")
    {
        var query = new GetTrendingTopicsQuery
        {
            Count = count,
            TimeFrame = timeFrame
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{name}/related")]
    public async Task<ActionResult<List<Tag>>> GetRelatedTags(string name)
    {
        var query = new GetRelatedTagsQuery { TagName = name };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<TagDto>>> GetUserTags(Guid userId)
    {
        var query = new GetUserTagsQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{name}/synonyms")]
    public async Task<ActionResult<List<TagSynonymDto>>> GetTagSynonyms(string name)
    {
        var query = new GetTagSynonymsQuery { TagName = name };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("{name}/synonyms")]
    public async Task<ActionResult> AddTagSynonym(string name, AddTagSynonymCommand command)
    {
        if (name != command.TagName)
            return BadRequest("Tag name mismatch");
        
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{name}/wiki")]
    public async Task<ActionResult<TagWikiDto>> GetTagWiki(string name)
    {
        var query = new GetTagWikiQuery { TagName = name };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("{name}/wiki")]
    public async Task<ActionResult> UpdateTagWiki(string name, UpdateTagWikiCommand command)
    {
        if (name != command.TagName)
            return BadRequest("Tag name mismatch");
        
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{name}/relationships")]
    public async Task<ActionResult<List<TagRelationshipDto>>> GetTagRelationships(string name)
    {
        var query = new GetTagRelationshipsQuery { TagName = name };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("relationships")]
    public async Task<ActionResult> AddTagRelationship(CreateTagRelationshipCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("relationships/{parentTag}/{childTag}")]
    public async Task<ActionResult> RemoveTagRelationship(string parentTag, string childTag)
    {
        var command = new RemoveTagRelationshipCommand 
        { 
            ParentTag = parentTag,
            ChildTag = childTag
        };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{name}/stats")]
    public async Task<ActionResult<TagStatsDto>> GetTagStats(string name)
    {
        var query = new GetTagStatsQuery { TagName = name };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("{name}/follow")]
    public async Task<ActionResult> FollowTag(string name)
    {
        var userId = GetUserId();
        var command = new FollowTagCommand
        {
            UserId = userId,
            TagName = name,
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{name}/follow")]
    public async Task<ActionResult> UnfollowTag(string name)
    {
        var userId = GetUserId();
        var command = new UnfollowTagCommand
        {
            UserId = userId,
            TagName = name,
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<TagDto>>> GetFollowedTags()
    {
        var userId = GetUserId();
        var query = new GetFollowedTagsQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{name}/moderation-history")]
    public async Task<ActionResult<List<TagModerationLogDto>>> GetTagModerationHistory(string name)
    {
        var query = new GetTagModerationHistoryQuery { TagName = name };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}

public record CreateTagCommand
{
    public string Name { get; init; }
    public string? Description { get; init; }
}

public record GetPostsByTagQuery : IRequest<Result<List<PostDto>>>
{
    public string TagName { get; init; }
    public string SortBy { get; init; } = "merit";
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}

public record GetRelatedTagsQuery : IRequest<Result<List<Tag>>>
{
    public string TagName { get; init; }
}