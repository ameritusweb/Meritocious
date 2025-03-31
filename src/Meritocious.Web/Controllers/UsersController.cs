using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Features.Users.Commands;
using Meritocious.Core.Features.Users.Queries;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Discovery.Queries;
using Meritocious.Core.Features.Notifications.Commands;
using Meritocious.Core.Features.Notifications.Queries;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserProfileDto>> Register(RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id}/profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(Guid id)
    {
        var query = new GetUserProfileQuery { UserId = id };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("top-contributors")]
    public async Task<ActionResult<List<UserProfileDto>>> GetTopContributors(
        [FromQuery] int count = 10,
        [FromQuery] string timeFrame = "all")
    {
        var query = new GetTopContributorsQuery
        {
            Count = count,
            TimeFrame = timeFrame
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}/posts")]
    public async Task<ActionResult<List<PostDto>>> GetUserPosts(Guid id)
    {
        var query = new GetPostsByUserQuery { UserId = id };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id}/notifications")]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
        Guid id,
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int? count = null)
    {
        var query = new GetUserNotificationsQuery
        {
            UserId = id,
            UnreadOnly = unreadOnly,
            Count = count
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id}/notifications/mark-read")]
    public async Task<ActionResult> MarkNotificationsAsRead(
        Guid id,
        [FromBody] List<Guid> notificationIds)
    {
        var command = new MarkNotificationsAsReadCommand
        {
            UserId = id,
            NotificationIds = notificationIds
        };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id}/recommended-posts")]
    public async Task<ActionResult<List<PostRecommendationDto>>> GetRecommendedPosts(
        Guid id,
        [FromQuery] int count = 10)
    {
        var query = new GetRecommendedPostsQuery
        {
            UserId = id,
            Count = count
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}