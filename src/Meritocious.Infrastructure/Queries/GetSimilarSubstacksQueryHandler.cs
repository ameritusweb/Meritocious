using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Features.Substacks.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetSimilarSubstacksQueryHandler : IRequestHandler<GetSimilarSubstacksQuery, List<SubstackDto>>
{
    private readonly MeritociousDbContext context;

    public GetSimilarSubstacksQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<List<SubstackDto>> Handle(GetSimilarSubstacksQuery request, CancellationToken cancellationToken)
    {
        var substackTopics = await context.Substacks
            .Where(s => s.Id.ToString() == request.SubstackId)
            .SelectMany(s => s.Topics.Select(t => t.ContentId))
            .ToListAsync(cancellationToken);

        return await context.Substacks
            .Where(s => s.Id.ToString() != request.SubstackId && s.Topics.Any(t => substackTopics.Contains(t.ContentId)))
            .OrderByDescending(s => s.Topics.Count(t => substackTopics.Contains(t.ContentId)))
            .ThenByDescending(s => s.AvgMeritScore)
            .Take(request.Limit)
            .Select(s => new SubstackDto
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Subdomain = s.Subdomain,
                CustomDomain = s.CustomDomain,
                AuthorName = s.AuthorName,
                Description = s.Description,
                LogoUrl = s.LogoUrl,
                CoverImageUrl = s.CoverImageUrl,
                TwitterHandle = s.TwitterHandle,
                CreatedAt = s.CreatedAt,
                LastUpdated = s.LastUpdated,
                FollowerCount = s.FollowerCount,
                PostCount = s.PostCount,
                ImportedPostCount = s.ImportedPostCount,
                IsVerified = s.IsVerified,
                Metrics = new SubstackMetricsDto
                {
                    TotalPosts = s.PostCount,
                    TotalImportedPosts = s.ImportedPostCount,
                    TotalRemixes = s.TotalRemixes,
                    TotalComments = s.TotalComments,
                    TotalViews = s.TotalViews,
                    UniqueViewers = s.UniqueViewers,
                    AvgPostLength = s.AvgPostLength,
                    AvgCommentLength = s.AvgCommentLength,
                    AvgMeritScore = s.AvgMeritScore,
                    LastPostDate = s.LastPostDate,
                    PostsLastWeek = s.PostsLastWeek,
                    PostsLastMonth = s.PostsLastMonth,
                    EngagementRate = s.EngagementRate,
                    GrowthRate = s.GrowthRate
                }
            })
            .ToListAsync(cancellationToken);
    }
}