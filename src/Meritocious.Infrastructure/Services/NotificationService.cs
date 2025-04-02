using Microsoft.AspNetCore.SignalR;
using Meritocious.Common.DTOs.Notifications;
using Meritocious.Web.Hubs;
using Meritocious.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Meritocious.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationAsync(Guid userId, NotificationDto notification)
    {
        try
        {
            // Store notification in database
            // TODO: Implement database storage

            // Send real-time notification
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("ReceiveNotification", notification);

            _logger.LogInformation(
                "Sent notification {Title} to user {UserId}", 
                notification.Title, 
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending notification {Title} to user {UserId}",
                notification.Title,
                userId);
            throw;
        }
    }

    public async Task SendNotificationToGroupAsync(string groupName, NotificationDto notification)
    {
        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("ReceiveNotification", notification);

            _logger.LogInformation(
                "Sent notification {Title} to group {GroupName}",
                notification.Title,
                groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending notification {Title} to group {GroupName}",
                notification.Title,
                groupName);
            throw;
        }
    }

    public async Task MarkAsReadAsync(Guid userId, string notificationId)
    {
        try
        {
            // Update database
            // TODO: Implement database update

            // Notify all user's connections
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("NotificationRead", notificationId);

            _logger.LogInformation(
                "Marked notification {NotificationId} as read for user {UserId}",
                notificationId,
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error marking notification {NotificationId} as read for user {UserId}",
                notificationId,
                userId);
            throw;
        }
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false)
    {
        // TODO: Implement database query
        // This is a mock implementation
        return new List<NotificationDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "New Comment",
                Message = "Someone commented on your post",
                Type = "Comment",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };
    }
}