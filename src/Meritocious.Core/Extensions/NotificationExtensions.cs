using Meritocious.Common.DTOs.Notifications;
using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public static class NotificationExtensions
    {
        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                ActionUrl = notification.Link,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }

        public static List<NotificationDto> ToDtoList(this IEnumerable<Notification> notifications)
        {
            return notifications.Select(n => n.ToDto()).ToList();
        }
    }
}
