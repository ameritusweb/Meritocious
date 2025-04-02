using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Features.Users.Commands;
using Meritocious.Core.Features.Users.Queries;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Discovery.Queries;
using Meritocious.Core.Features.Notifications.Commands;
using Meritocious.Core.Features.Notifications.Queries;
using Meritocious.Core.Features.Posts.Queries;
using Meritocious.Common.DTOs.Notifications;

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

    [HttpGet("me/profile")]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUserProfile()
    {
        var userId = GetUserId();
        var query = new GetUserProfileQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id}/merit-score")]
    public async Task<ActionResult<MeritScoreDto>> GetUserMeritScore(Guid id)
    {
        var query = new GetUserMeritScoreQuery { UserId = id };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id}/merit-history")]
    public async Task<ActionResult<List<ReputationSnapshot>>> GetUserMeritHistory(
        Guid id,
        [FromQuery] string timeFrame = "monthly",
        [FromQuery] DateTime? start = null,
        [FromQuery] DateTime? end = null)
    {
        var query = new GetUserMeritHistoryQuery 
        { 
            UserId = id,
            TimeFrame = timeFrame,
            StartDate = start,
            EndDate = end
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("me/profile")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(UpdateUserProfileCommand command)
    {
        var userId = GetUserId();
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("me/settings")]
    public async Task<ActionResult<UserSettingsDto>> UpdateSettings(UpdateUserSettingsCommand command)
    {
        var userId = GetUserId();
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id}/contributions")]
    public async Task<ActionResult<List<ContributionSummaryDto>>> GetUserContributions(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserContributionsQuery 
        { 
            UserId = id,
            Page = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}