using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Discovery.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Content;
    using Meritocious.Core.Exceptions;
    using Meritocious.Core.Interfaces;

    public record GetRecommendedPostsQuery : IRequest<List<PostRecommendationDto>>
    {
        public Guid UserId { get; init; }
        public int Count { get; init; } = 10;
        public List<string> ExcludedPostIds { get; init; } = new();
    }

    public class PostRecommendationDto
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string AuthorUsername { get; set; }
        public decimal MeritScore { get; set; }
        public List<string> Tags { get; set; } = new();
        public string RecommendationReason { get; set; }
        public decimal RelevanceScore { get; set; }
    }
}