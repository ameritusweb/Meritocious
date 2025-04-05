using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Events
{
    using MediatR;

    public class PostCreatedEvent : INotification
    {
        public string PostId { get; }
        public string AuthorId { get; }
        public DateTime CreatedAt { get; }

        public PostCreatedEvent(string postId, string authorId)
        {
            PostId = postId;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
