using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ContentModerationEvent : BaseEntity
    {
        public Guid ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public ModerationAction Action { get; private set; }
        public string Reason { get; private set; }
        public bool IsAutomated { get; private set; }
        public Guid? ModeratorId { get; private set; }
        public User Moderator { get; private set; }
        public DateTime ModeratedAt { get; private set; }

        private ContentModerationEvent() { }

        public static ContentModerationEvent Create(
            Guid contentId,
            ContentType contentType,
            ModerationAction action,
            string reason,
            bool isAutomated,
            Guid? moderatorId = null)
        {
            return new ContentModerationEvent
            {
                ContentId = contentId,
                ContentType = contentType,
                Action = action,
                Reason = reason,
                IsAutomated = isAutomated,
                ModeratorId = moderatorId,
                ModeratedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
