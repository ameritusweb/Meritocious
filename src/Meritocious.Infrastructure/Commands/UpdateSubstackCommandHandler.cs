using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Features.Substacks.Commands;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Exceptions;

namespace Meritocious.Infrastructure.Commands;

public class UpdateSubstackCommandHandler : IRequestHandler<UpdateSubstackCommand, SubstackDto>
{
    private readonly MeritociousDbContext _context;

    public UpdateSubstackCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<SubstackDto> Handle(UpdateSubstackCommand request, CancellationToken cancellationToken)
    {
        var substack = await _context.Substacks
            .FirstOrDefaultAsync(s => s.Id == request.SubstackId, cancellationToken);

        if (substack == null)
        {
            throw new ResourceNotFoundException($"Substack with ID {request.SubstackId} not found");
        }

        substack.Name = request.Name;
        substack.CustomDomain = request.CustomDomain;
        substack.Description = request.Description;
        substack.LogoUrl = request.LogoUrl;
        substack.CoverImageUrl = request.CoverImageUrl;
        substack.TwitterHandle = request.TwitterHandle;
        substack.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SubstackDto
        {
            Id = substack.Id,
            Name = substack.Name,
            Subdomain = substack.Subdomain,
            CustomDomain = substack.CustomDomain,
            AuthorName = substack.AuthorName,
            Description = substack.Description,
            LogoUrl = substack.LogoUrl,
            CoverImageUrl = substack.CoverImageUrl,
            TwitterHandle = substack.TwitterHandle,
            CreatedAt = substack.CreatedAt,
            LastUpdated = substack.LastUpdated,
            FollowerCount = substack.FollowerCount,
            PostCount = substack.PostCount,
            ImportedPostCount = substack.ImportedPostCount,
            IsVerified = substack.IsVerified,
            Metrics = new SubstackMetricsDto
            {
                TotalPosts = substack.PostCount,
                TotalImportedPosts = substack.ImportedPostCount,
                TotalRemixes = substack.TotalRemixes,
                TotalComments = substack.TotalComments,
                TotalViews = substack.TotalViews,
                UniqueViewers = substack.UniqueViewers,
                AvgPostLength = substack.AvgPostLength,
                AvgCommentLength = substack.AvgCommentLength,
                AvgMeritScore = substack.AvgMeritScore,
                LastPostDate = substack.LastPostDate,
                PostsLastWeek = substack.PostsLastWeek,
                PostsLastMonth = substack.PostsLastMonth,
                EngagementRate = substack.EngagementRate,
                GrowthRate = substack.GrowthRate
            }
        };
    }
}