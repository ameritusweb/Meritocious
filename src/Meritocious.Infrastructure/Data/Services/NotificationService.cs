using Meritocious.Core.Features.Notifications.Models;
using Meritocious.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Meritocious.Infrastructure.Data.Services
{
    public class NotificationService : INotificationService
    {
        private readonly MeritociousDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            MeritociousDbContext context,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Notification> SendNotificationAsync(Notification notification)
        {
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                // In a real implementation, we might also:
                // 1. Send a push notification
                // 2. Send an email
                // 3. Trigger a websocket event

                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", notification.UserId);
                throw;
            }
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(
            Guid userId,
            bool unreadOnly = false,
            int? count = null)
        {
            try
            {
                var query = _context.Notifications
                    .Where(n => n.UserId == userId);

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
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task MarkNotificationsAsReadAsync(Guid userId, List<Guid> notificationIds)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && notificationIds.Contains(n.Id))
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.MarkAsRead();
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notifications as read for user {UserId}", userId);
                throw;
            }
        }
    }
}
