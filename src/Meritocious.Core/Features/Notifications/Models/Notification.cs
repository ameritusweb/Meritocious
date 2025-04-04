using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Notifications.Models
{
    using Meritocious.Core.Entities;

    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Type { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string? ActionUrl { get; private set; }
        public bool IsRead { get; private set; }

        private Notification()
        {
        }

        public static Notification Create(
            Guid userId,
            string type,
            string title,
            string message,
            string? actionUrl = null)
        {
            return new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsRead()
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}