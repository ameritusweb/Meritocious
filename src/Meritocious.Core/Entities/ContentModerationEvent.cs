using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ContentModerationEvent : BaseEntity<ContentModerationEvent>
    {
        public string ContentId { get; private set; }

        public ContentType ContentType { get; private set; }

        public ModerationAction Action { get; private set; }

        public string ActionId { get; private set; }

        public string Reason { get; private set; }

        public bool IsAutomated { get; private set; }

        public string ModeratorId { get; private set; }

        public User Moderator { get; private set; }

        public DateTime ModeratedAt { get; private set; }

        private ContentModerationEvent()
        {
        }

        public static ContentModerationEvent Create(
            string contentId,
            ContentType contentType,
            ModerationAction action,
            string reason,
            bool isAutomated,
            string? moderatorId = null)
        {
            return new ContentModerationEvent
            {
                ContentId = contentId,
                ContentType = contentType,
                Action = action,
                Reason = reason,
                IsAutomated = isAutomated,
                ModeratorId = moderatorId.ToString(),
                ModeratedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
