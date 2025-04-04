using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Content
{
    public class PostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string AuthorUsername { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public object AuthorId { get; set; }
        public object AuthorName { get; set; }
        public object AuthorAvatar { get; set; }
        public object Tags { get; set; }
        public object CommentCount { get; set; }
        public object ViewCount { get; set; }
        public object EngagementScore { get; set; }
        public object TrendingScore { get; set; }
    }
}
