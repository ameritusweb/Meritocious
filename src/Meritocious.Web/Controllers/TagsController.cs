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
using Meritocious.Infrastructure.Queries;
using Meritocious.Core.Features.Tags.Models;

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
        var tag = await _tagService.CreateTagAsync(command.Name, (TagCategory)command.Category, command.Description);
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
    public async Task<ActionResult> AddTagToPost(string name, Guid postId, int category)
    {
        await _tagService.AddTagToPostAsync(postId, name, (TagCategory)category);
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
    public async Task<ActionResult<List<Tag>>> GetRelatedTags(string id)
    {
        var query = new GetRelatedTagsQuery { TagId = id };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<TagDto>>> GetUserTags(Guid userId)
    {
        var query = new GetUserTagsQuery(userId.ToString());
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{name}/synonyms")]
    public async Task<ActionResult<List<TagSynonymDto>>> GetTagSynonyms(string id)
    {
        var query = new GetTagSynonymsQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{name}/synonyms")]
    public async Task<ActionResult> AddTagSynonym(string id, AddTagSynonymCommand command)
    {
        if (id != command.SourceTagId)
        {
            return BadRequest("Tag ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{name}/wiki")]
    public async Task<ActionResult<TagWikiDto>> GetTagWiki(string id)
    {
        var query = new GetTagWikiQuery { TagId = id };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("{name}/wiki")]
    public async Task<ActionResult> UpdateTagWiki(string id, UpdateTagWikiCommand command)
    {
        command.TagId = id;

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{name}/relationships")]
    public async Task<ActionResult<List<TagRelationshipDto>>> GetTagRelationships(string id)
    {
        var query = new GetTagRelationshipsQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("relationships")]
    public async Task<ActionResult> AddTagRelationship(CreateTagRelationshipCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("relationships/{parentTagId}/{childTagId}")]
    public async Task<ActionResult> RemoveTagRelationship(string parentTagId, string childTagId)
    {
        var command = new RemoveTagRelationshipCommand(parentTagId, childTagId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{name}/stats")]
    public async Task<ActionResult<TagDto>> GetTagStats(string id)
    {
        var query = new GetTagStatsQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{name}/follow")]
    public async Task<ActionResult> FollowTag(string id)
    {
        var userId = GetUserId();
        var command = new FollowTagCommand(userId, id);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{name}/follow")]
    public async Task<ActionResult> UnfollowTag(string id)
    {
        var userId = GetUserId();
        var command = new UnfollowTagCommand(userId, id);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<TagDto>>> GetFollowedTags()
    {
        var userId = GetUserId();
        var query = new GetFollowedTagsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{name}/moderation-history")]
    public async Task<ActionResult<List<TagModerationLogDto>>> GetTagModerationHistory(string id)
    {
        var query = new GetTagModerationHistoryQuery(id, 1, 20);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

public record CreateTagCommand
{
    public string Name { get; init; }
    public int Category { get; init; }
    public string Description { get; init; }
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
    public string TagId { get; init; }
}