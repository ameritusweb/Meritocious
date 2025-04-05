using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Features.Substacks.Commands;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Exceptions;

namespace Meritocious.Infrastructure.Commands;

public class CreateSubstackCommandHandler : IRequestHandler<CreateSubstackCommand, SubstackDto>
{
    private readonly MeritociousDbContext context;

    public CreateSubstackCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<SubstackDto> Handle(CreateSubstackCommand request, CancellationToken cancellationToken)
    {
        var existingSubstack = await context.Substacks
            .FirstOrDefaultAsync(s => s.Subdomain == request.Subdomain, cancellationToken);

        if (existingSubstack != null)
        {
            throw new DuplicateResourceException($"Substack with subdomain {request.Subdomain} already exists");
        }

        var substack = new Core.Entities.Substack
        {
            Name = request.Name,
            Subdomain = request.Subdomain,
            CustomDomain = request.CustomDomain,
            AuthorName = request.AuthorName,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            CoverImageUrl = request.CoverImageUrl,
            TwitterHandle = request.TwitterHandle,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            IsVerified = false
        };

        context.Substacks.Add(substack);
        await context.SaveChangesAsync(cancellationToken);

        return new SubstackDto
        {
            Id = substack.Id.ToString(),
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
            FollowerCount = 0,
            PostCount = 0,
            ImportedPostCount = 0,
            IsVerified = substack.IsVerified,
            Metrics = new SubstackMetricsDto
            {
                TotalPosts = 0,
                TotalImportedPosts = 0,
                TotalRemixes = 0,
                TotalComments = 0,
                TotalViews = 0,
                UniqueViewers = 0,
                AvgPostLength = 0,
                AvgCommentLength = 0,
                AvgMeritScore = 0,
                LastPostDate = DateTime.UtcNow,
                PostsLastWeek = 0,
                PostsLastMonth = 0,
                EngagementRate = 0,
                GrowthRate = 0
            }
        };
    }
}