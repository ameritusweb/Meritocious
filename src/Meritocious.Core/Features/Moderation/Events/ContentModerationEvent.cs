using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Moderation.Events
{
    using MediatR;
    using Meritocious.Common.Enums;

    public record ContentModeratedEvent : INotification
    {
        public Guid ContentId { get; }
        public ContentType ContentType { get; }
        public ModerationAction Action { get; }
        public string Reason { get; }
        public bool IsAutomated { get; }
        public Guid? ModeratorId { get; }
        public DateTime ModeratedAt { get; }

        public ContentModeratedEvent(
            Guid contentId,
            ContentType contentType,
            ModerationAction action,
            string reason,
            bool isAutomated,
            Guid? moderatorId)
        {
            ContentId = contentId;
            ContentType = contentType;
            Action = action;
            Reason = reason;
            IsAutomated = isAutomated;
            ModeratorId = moderatorId;
            ModeratedAt = DateTime.UtcNow;
        }
    }
}