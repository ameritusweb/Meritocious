using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Commands;
using Meritocious.Core.Features.Posts.Commands;
using Meritocious.Core.Features.Posts.Queries;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Comments.Queries;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
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
}