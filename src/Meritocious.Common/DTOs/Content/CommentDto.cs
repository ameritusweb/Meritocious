using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Content
{
    public class CommentListResponse
    {
        public List<CommentDto> Comments { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class CommentDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string PostId { get; set; }
        public string PostTitle { get; set; }
        public string AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public string? ParentCommentId { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
        public int LikesCount { get; set; }
        public bool HasLiked { get; set; }
    }
}
