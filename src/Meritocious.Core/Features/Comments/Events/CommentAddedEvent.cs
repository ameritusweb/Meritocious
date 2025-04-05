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
        public string CommentId { get; }
        public string PostId { get; }
        public string AuthorId { get; }
        public DateTime CreatedAt { get; }

        public CommentAddedEvent(string commentId, string postId, string authorId)
        {
            CommentId = commentId;
            PostId = postId;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}