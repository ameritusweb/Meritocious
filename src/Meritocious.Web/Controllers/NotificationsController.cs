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
    public NotificationsController(IMediator mediator) : base(mediator)
    {
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

        var notifications = await Mediator.Send(query);
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

        var notifications = await Mediator.Send(query);
        return Ok(notifications.Count);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = GetUserId();
        var command = new MarkNotificationsAsReadCommand
        {
            UserId = userId,
            NotificationIds = new List<Guid> { id }
        };

        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        var notifications = await Mediator.Send(new GetUserNotificationsQuery
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

            await Mediator.Send(command);
        }

        return Ok();
    }
}