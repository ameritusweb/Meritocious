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
        public Guid PostId { get; }
        public Guid AuthorId { get; }
        public DateTime CreatedAt { get; }

        public PostCreatedEvent(Guid postId, Guid authorId)
        {
            PostId = postId;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
