using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Discovery.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Content;
    using Meritocious.Core.Interfaces;

    public record GetTrendingTopicsQuery : IRequest<List<TrendingTopicDto>>
    {
        public int Count { get; init; } = 10;
        public string TimeFrame { get; init; } = "day"; // hour, day, week, month
    }

    public class TrendingTopicDto
    {
        public string Topic { get; set; }
        public int PostCount { get; set; }
        public int CommentCount { get; set; }
        public decimal AverageMeritScore { get; set; }
        public List<string> RelatedTags { get; set; } = new();
        public List<PostSummaryDto> TopPosts { get; set; } = new();
    }
}
