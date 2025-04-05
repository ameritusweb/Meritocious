using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Notifications.Events
{
    using MediatR;

    public record NotificationEvent : INotification
    {
        public string UserId { get; }
        public string Type { get; }
        public string Title { get; }
        public string Message { get; }
        public string? ActionUrl { get; }
        public DateTime CreatedAt { get; }

        public NotificationEvent(
            string userId,
            string type,
            string title,
            string message,
            string? actionUrl = null)
        {
            UserId = userId;
            Type = type;
            Title = title;
            Message = message;
            ActionUrl = actionUrl;
            CreatedAt = DateTime.UtcNow;
        }
    }
}