﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Extensions;
using Meritocious.Core.Features.Comments.Commands;
using Meritocious.Core.Features.Comments.Queries;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Reporting.Commands;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ApiControllerBase
{
    private readonly IMediator mediator;

    public CommentsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment(AddCommentCommand command)
    {
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(string id, UpdateCommentCommand command)
    {
        if (id != command.CommentId)
        {
            return BadRequest("ID mismatch");
        }

        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(string id)
    {
        var command = new DeleteCommentCommand { CommentId = id };
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id}/replies")]
    public async Task<ActionResult<List<CommentDto>>> GetReplies(
        string id,
        [FromQuery] string sortBy = "merit",
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        var query = new GetCommentRepliesQuery
        {
            CommentId = id,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
        };
        var result = await mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("{id}/report")]
    public async Task<ActionResult> ReportComment(string id, ReportContentCommand command)
    {
        if (id != command.ContentId)
        {
            return BadRequest("ID mismatch");
        }

        var result = await mediator.Send(command);
        return HandleResult(result);
    }
}