using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetFollowedTagsQueryHandler : IRequestHandler<GetFollowedTagsQuery, IEnumerable<TagDto>>
{
    private readonly MeritociousDbContext _context;

    public GetFollowedTagsQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagDto>> Handle(GetFollowedTagsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(u => u.Id == request.UserId)
            .SelectMany(u => u.FollowedTags)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                UsageCount = t.UsageCount,
                FollowerCount = t.FollowerCount,
                CreatedAt = t.CreatedAt,
                LastModified = t.LastModified,
                IsModerated = t.IsModerated
            })
            .ToListAsync(cancellationToken);
    }
}