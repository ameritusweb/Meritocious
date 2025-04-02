using Microsoft.AspNetCore.SignalR;
using Meritocious.Common.DTOs.Notifications;

namespace Meritocious.Web.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
        await base.OnConnectedAsync();
    }

    public async Task MarkAsRead(string notificationId)
    {
        // Client will call this to mark a notification as read
        // We'll broadcast the update to all user's connections
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group($"user-{userId}").SendAsync("NotificationRead", notificationId);
        }
    }
}