using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meritocious.Infrastructure.Data.Services
{
    public class NotificationService : INotificationService
    {
        private readonly MeritociousDbContext context;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(
            MeritociousDbContext context,
            ILogger<NotificationService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Notification> SendNotificationAsync(Notification notification)
        {
            try
            {
                await context.Notifications.AddAsync(notification);
                await context.SaveChangesAsync();

                // In a real implementation, we might also:
                // 1. Send a push notification
                // 2. Send an email
                // 3. Trigger a websocket event
                return notification;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending notification to user {UserId}", notification.UserId);
                throw;
            }
        }

        public async Task<List<Meritocious.Core.Entities.Notification>> GetUserNotificationsAsync(
            Guid userId,
            bool unreadOnly = false,
            int? count = null)
        {
            try
            {
                var query = context.Notifications
                    .Where(n => n.UserId == userId.ToString());

                if (unreadOnly)
                {
                    query = query.Where(n => !n.IsRead);
                }

                query = query.OrderByDescending(n => n.CreatedAt);

                if (count.HasValue)
                {
                    query = query.Take(count.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task MarkNotificationsAsReadAsync(Guid userId, List<Guid> notificationIds)
        {
            try
            {
                var notifications = await context.Notifications
                    .Where(n => n.UserId == userId.ToString() && notificationIds.Contains(n.Id))
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.MarkAsRead();
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error marking notifications as read for user {UserId}", userId);
                throw;
            }
        }
    }
}
