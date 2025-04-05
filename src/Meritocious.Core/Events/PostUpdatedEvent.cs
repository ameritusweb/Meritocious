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
        public string PostId { get; }
        public string EditorId { get; }
        public DateTime UpdatedAt { get; }

        public PostUpdatedEvent(string postId, string editorId)
        {
            PostId = postId;
            EditorId = editorId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
