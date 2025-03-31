using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Events
{
    using MediatR;

    public record CommentAddedEvent : INotification
    {
        public Guid CommentId { get; }
        public Guid PostId { get; }
        public Guid AuthorId { get; }
        public DateTime CreatedAt { get; }

        public CommentAddedEvent(Guid commentId, Guid postId, Guid authorId)
        {
            CommentId = commentId;
            PostId = postId;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}