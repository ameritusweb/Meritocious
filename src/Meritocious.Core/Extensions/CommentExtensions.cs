using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public static class CommentExtensions
    {
        public static CommentDto ToDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                PostTitle = comment.Post?.Title ?? "Unknown Post",
                AuthorId = comment.AuthorId,
                AuthorUsername = comment.Author?.Username ?? "Unknown User",
                ParentCommentId = comment.ParentCommentId,
                MeritScore = comment.MeritScore,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                // Only include immediate replies if they're loaded
                Replies = comment.Replies?.Select(r => r.ToDto()).ToList() ?? new List<CommentDto>()
            };
        }

        public static List<CommentDto> ToDtoList(this IEnumerable<Comment> comments)
        {
            return comments.Select(c => c.ToDto()).ToList();
        }
    }
}
