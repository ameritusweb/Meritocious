using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Posts.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries
{
    public class GetTrendingPostsQueryHandler : IRequestHandler<GetTrendingPostsQuery, List<PostSummaryDto>>
    {
        private readonly MeritociousDbContext _context;
        private readonly ILogger<GetTrendingPostsQueryHandler> _logger;

        public GetTrendingPostsQueryHandler(
            MeritociousDbContext context,
            ILogger<GetTrendingPostsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PostSummaryDto>> Handle(
            GetTrendingPostsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var cutoffDate = request.TimeFrame.ToLower() switch
                {
                    "day" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    _ => DateTime.UtcNow.AddDays(-1)
                };

                var query = _context.Posts
                    .Include(p => p.Author)
                    .Include(p => p.Tags)
                    .Where(p => !p.IsDeleted &&
                               p.CreatedAt >= cutoffDate &&
                               p.MeritScore >= request.MinMeritScore);

                if (!string.IsNullOrEmpty(request.Category))
                {
                    query = query.Where(p => p.Tags.Any(t => t.Name == request.Category));
                }

                // Calculate trending score based on:
                // - Merit score (40%)
                // - Recent engagement (30%)
                // - View count (20%)
                // - Recency (10%)
                var posts = await query
                    .Select(p => new
                    {
                        Post = p,
                        TrendingScore = 
                            (p.MeritScore * 0.4m) +
                            (p.EngagementScore * 0.3m) +
                            (p.ViewCount * 0.2m / 1000) + // Normalize views
                            (0.1m * (1 - ((DateTime.UtcNow - p.CreatedAt).TotalHours / 24))) // Recency factor
                    })
                    .OrderByDescending(x => x.TrendingScore)
                    .Take(request.Limit)
                    .Select(x => new PostSummaryDto
                    {
                        Id = x.Post.Id,
                        Title = x.Post.Title,
                        AuthorId = x.Post.AuthorId,
                        AuthorName = x.Post.Author.Username,
                        AuthorAvatar = x.Post.Author.AvatarUrl,
                        CreatedAt = x.Post.CreatedAt,
                        MeritScore = x.Post.MeritScore,
                        Tags = x.Post.Tags.Select(t => t.Name).ToList(),
                        CommentCount = x.Post.CommentCount,
                        ViewCount = x.Post.ViewCount,
                        EngagementScore = x.Post.EngagementScore,
                        TrendingScore = x.TrendingScore
                    })
                    .ToListAsync(cancellationToken);

                return posts;
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Error getting trending posts");
                throw;
            }
        }
    }
}