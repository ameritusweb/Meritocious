using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> SendNotificationAsync(Notification notification);
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, int? count = null);
        Task MarkNotificationsAsReadAsync(Guid userId, List<Guid> notificationIds);
    }
}
