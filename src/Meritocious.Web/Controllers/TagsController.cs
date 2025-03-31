using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Discovery.Queries;
using Meritocious.Core.Results;

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