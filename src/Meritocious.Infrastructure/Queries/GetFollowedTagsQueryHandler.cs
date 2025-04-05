using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetFollowedTagsQueryHandler : IRequestHandler<GetFollowedTagsQuery, IEnumerable<TagDto>>
{
    private readonly MeritociousDbContext context;

    public GetFollowedTagsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TagDto>> Handle(GetFollowedTagsQuery request, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.Id == request.UserId)
            .SelectMany(u => u.FollowedTags)
            .Select(t => new TagDto
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                Description = t.Description,
                UsageCount = t.UseCount,
                FollowerCount = t.FollowerCount,
                CreatedAt = t.CreatedAt,
                LastModified = t.UpdatedAt,
                IsModerated = t.Status == Core.Features.Tags.Models.TagStatus.Moderated
            })
            .ToListAsync(cancellationToken);
    }
}