using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Features.Substacks.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetRecommendedSubstacksQueryHandler : IRequestHandler<GetRecommendedSubstacksQuery, List<SubstackDto>>
{
    private readonly MeritociousDbContext context;

    public GetRecommendedSubstacksQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<List<SubstackDto>> Handle(GetRecommendedSubstacksQuery request, CancellationToken cancellationToken)
    {
        var userInterests = await context.UserTopicPreferences
            .Where(p => p.UserId == request.UserId)
            .Select(p => p.TopicId)
            .ToListAsync(cancellationToken);

        return await context.Substacks
            .Where(s => s.Topics.Any(t => userInterests.Contains(t.TopicId)))
            .OrderByDescending(s => s.AvgMeritScore)
            .Take(request.Limit)
            .Select(s => new SubstackDto
            {
                Id = s.Id,
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