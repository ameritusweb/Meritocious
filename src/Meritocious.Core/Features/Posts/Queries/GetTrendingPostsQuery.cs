using MediatR;
using Meritocious.Common.DTOs.Content;

namespace Meritocious.Core.Features.Posts.Queries
{
    public record GetTrendingPostsQuery(
        string TimeFrame = "day",  // day, week, month
        string? Category = null,
        int Limit = 10,
        decimal MinMeritScore = 0.0m)
        : IRequest<List<PostSummaryDto>>;
}