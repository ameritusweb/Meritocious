using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Content
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid PostId { get; set; }
        public string PostTitle { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public Guid? ParentCommentId { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}
