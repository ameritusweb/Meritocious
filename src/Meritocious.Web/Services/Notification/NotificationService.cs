using Meritocious.Common.DTOs.Notifications;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Meritocious.Infrastructure.Data;
using Meritocious.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Meritocious.Web.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly ILogger<NotificationService> logger;
        private readonly MeritociousDbContext dbContext;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            MeritociousDbContext dbContext,
            ILogger<NotificationService> logger)
        {
            this.hubContext = hubContext;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task SendNotificationAsync(Guid userId, NotificationDto notification)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString());

                // Create and store notification in database
                var entity = Meritocious.Core.Entities.Notification.Create(
                    user,
                    notification.Type,
                    notification.Title,
                    notification.Message,
                    notification.ActionUrl);

                dbContext.Notifications.Add(entity);
                await dbContext.SaveChangesAsync();

                // Update DTO with generated ID
                notification.Id = entity.Id;

                // Send real-time notification
                await hubContext.Clients
                    .Group($"user-{userId}")
                    .SendAsync("ReceiveNotification", notification);

                logger.LogInformation(
                    "Sent notification {Title} to user {UserId}",
                    notification.Title,
                    userId);
            }
            catch (Exception ex)
            {
                logger.LogError(
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
                await hubContext.Clients
                    .Group(groupName)
                    .SendAsync("ReceiveNotification", notification);

                logger.LogInformation(
                    "Sent notification {Title} to group {GroupName}",
                    notification.Title,
                    groupName);
            }
            catch (Exception ex)
            {
                logger.LogError(
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
                var notification = await dbContext.Notifications
                    .FirstOrDefaultAsync(n => n.Id == Guid.Parse(notificationId) && n.UserId == userId.ToString());

                if (notification == null)
                {
                    throw new ArgumentException("Notification not found or unauthorized");
                }

                notification.MarkAsRead();
                await dbContext.SaveChangesAsync();

                // Notify all user's connections
                await hubContext.Clients
                    .Group($"user-{userId}")
                    .SendAsync("NotificationRead", notificationId);

                logger.LogInformation(
                    "Marked notification {NotificationId} as read for user {UserId}",
                    notificationId,
                    userId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error marking notification {NotificationId} as read for user {UserId}",
                    notificationId,
                    userId);
                throw;
            }
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false)
        {
            var query = dbContext.Notifications
                .Where(n => n.UserId == userId.ToString());

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
                UserId = Guid.Parse(n.UserId),
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ActionUrl = n.Link,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public Task MarkNotificationsAsReadAsync(Guid userId, List<Guid> notificationIds)
        {
            throw new NotImplementedException(); // TODO: Implement this
        }

        public Task<Core.Entities.Notification> SendNotificationAsync(Core.Entities.Notification notification)
        {
            throw new NotImplementedException(); // TODO: Implement this
        }

        Task<List<Core.Entities.Notification>> INotificationService.GetUserNotificationsAsync(Guid userId, bool unreadOnly, int? count)
        {
            throw new NotImplementedException(); // TODO: Implement this
        }
    }
}
