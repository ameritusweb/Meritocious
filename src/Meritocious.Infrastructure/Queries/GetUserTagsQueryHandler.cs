using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetUserTagsQueryHandler : IRequestHandler<GetUserTagsQuery, IEnumerable<TagDto>>
{
    private readonly MeritociousDbContext _context;

    public GetUserTagsQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagDto>> Handle(GetUserTagsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .Where(p => p.AuthorId == request.UserId)
            .SelectMany(p => p.Tags)
            .GroupBy(t => t.Id)
            .Select(g => new TagDto
            {
                Id = g.Key,
                Name = g.First().Name,
                Description = g.First().Description,
                UsageCount = g.Count(),
                FollowerCount = g.First().FollowerCount,
                CreatedAt = g.First().CreatedAt,
                LastModified = g.First().LastModified,
                IsModerated = g.First().IsModerated
            })
            .OrderByDescending(t => t.UsageCount)
            .ToListAsync(cancellationToken);
    }
}