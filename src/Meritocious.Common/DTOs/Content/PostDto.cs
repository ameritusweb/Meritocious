using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Content
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? ParentPostId { get; set; }
        public decimal MeritScore { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}