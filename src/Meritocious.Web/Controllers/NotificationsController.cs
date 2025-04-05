using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Common.DTOs.Notifications;
using Meritocious.Core.Features.Notifications.Queries;
using Meritocious.Core.Features.Notifications.Commands;
using MediatR;

namespace Meritocious.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ApiControllerBase
{
    private readonly IMediator mediator;
    public NotificationsController(IMediator mediator) : base()
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
        [FromQuery] bool unreadOnly = false)
    {
        var userId = GetUserId();
        var query = new GetUserNotificationsQuery
        {
            UserId = userId,
            UnreadOnly = unreadOnly
        };

        var notifications = await mediator.Send(query);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = GetUserId();
        var query = new GetUserNotificationsQuery
        {
            UserId = userId,
            UnreadOnly = true
        };

        var notifications = await mediator.Send(query);
        return Ok(notifications.Count);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var userId = GetUserId();
        var command = new MarkNotificationsAsReadCommand
        {
            UserId = userId,
            NotificationIds = new List<string> { id }
        };

        await mediator.Send(command);
        return Ok();
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        var notifications = await mediator.Send(new GetUserNotificationsQuery
        {
            UserId = userId,
            UnreadOnly = true
        });

        if (notifications.Any())
        {
            var command = new MarkNotificationsAsReadCommand
            {
                UserId = userId,
                NotificationIds = notifications.Select(n => n.Id).ToList()
            };

            await mediator.Send(command);
        }

        return Ok();
    }
}