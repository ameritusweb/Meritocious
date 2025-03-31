using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Events
{
    public class PostUpdatedEvent : INotification
    {
        public Guid PostId { get; }
        public Guid EditorId { get; }
        public DateTime UpdatedAt { get; }

        public PostUpdatedEvent(Guid postId, Guid editorId)
        {
            PostId = postId;
            EditorId = editorId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
