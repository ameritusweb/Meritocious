using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Meritocious.Common.DTOs.Notifications;
using Meritocious.Core.Features.Notifications.Models;
using Meritocious.Core.Interfaces;
using Meritocious.Infrastructure.Data;
using Meritocious.Web.Hubs;

namespace Meritocious.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;
    private readonly MeritociousDbContext _dbContext;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        MeritociousDbContext dbContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SendNotificationAsync(Guid userId, NotificationDto notification)
    {
        try
        {
            // Create and store notification in database
            var entity = Notification.Create(
                userId,
                notification.Type,
                notification.Title,
                notification.Message,
                notification.ActionUrl
            );

            _dbContext.Notifications.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Update DTO with generated ID
            notification.Id = entity.Id;

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
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == Guid.Parse(notificationId) && n.UserId == userId);

            if (notification == null)
            {
                throw new ArgumentException("Notification not found or unauthorized");
            }

            notification.MarkAsRead();
            await _dbContext.SaveChangesAsync();

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
        try
        {
            var query = _dbContext.Notifications
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50) // Limit to recent 50 notifications
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ActionUrl = n.ActionUrl,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
    }
}