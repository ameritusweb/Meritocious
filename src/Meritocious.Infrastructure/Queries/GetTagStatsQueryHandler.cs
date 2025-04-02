using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetTagStatsQueryHandler : IRequestHandler<GetTagStatsQuery, TagDto>
{
    private readonly MeritociousDbContext _context;

    public GetTagStatsQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<TagDto> Handle(GetTagStatsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tags
            .Where(t => t.Id == request.TagId)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}