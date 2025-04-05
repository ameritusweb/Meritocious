using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetTagStatsQueryHandler : IRequestHandler<GetTagStatsQuery, TagDto>
{
    private readonly MeritociousDbContext context;

    public GetTagStatsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<TagDto> Handle(GetTagStatsQuery request, CancellationToken cancellationToken)
    {
        return await context.Tags
            .Where(t => t.Id.ToString() == request.TagId)
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