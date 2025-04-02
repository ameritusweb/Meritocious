using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Features.Substacks.Queries;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Exceptions;

namespace Meritocious.Infrastructure.Queries;

public class GetSubstackMetricsQueryHandler : IRequestHandler<GetSubstackMetricsQuery, SubstackMetricsDto>
{
    private readonly MeritociousDbContext _context;

    public GetSubstackMetricsQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<SubstackMetricsDto> Handle(GetSubstackMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _context.Substacks
            .Where(s => s.Id == request.SubstackId)
            .Select(s => new SubstackMetricsDto
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
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (metrics == null)
        {
            throw new ResourceNotFoundException($"Substack with ID {request.SubstackId} not found");
        }

        return metrics;
    }
}