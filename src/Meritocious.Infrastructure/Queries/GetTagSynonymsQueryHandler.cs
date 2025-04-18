using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetTagSynonymsQueryHandler : IRequestHandler<GetTagSynonymsQuery, IEnumerable<TagSynonymDto>>
{
    private readonly MeritociousDbContext context;

    public GetTagSynonymsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TagSynonymDto>> Handle(GetTagSynonymsQuery request, CancellationToken cancellationToken)
    {
        return await context.TagSynonyms
            .Where(s => s.SourceTagId.ToString() == request.TagId || s.TargetTagId.ToString() == request.TagId)
            .Select(s => new TagSynonymDto
            {
                SourceTagId = s.SourceTagId.ToString(),
                TargetTagId = s.TargetTagId.ToString(),
                CreatedBy = s.CreatedBy.Id.ToString(),
                CreatedAt = s.CreatedAt,
                ApprovedBy = s.ApprovedBy.Id.ToString(),
                ApprovedAt = s.ApprovedAt,
                
                // TODO: Create status.
                // Status = s.Status
            })
            .ToListAsync(cancellationToken);
    }
}