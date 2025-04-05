using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetTagRelationshipsQueryHandler : IRequestHandler<GetTagRelationshipsQuery, IEnumerable<TagRelationshipDto>>
{
    private readonly MeritociousDbContext context;

    public GetTagRelationshipsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TagRelationshipDto>> Handle(GetTagRelationshipsQuery request, CancellationToken cancellationToken)
    {
        var relationships = await context.TagRelationships
            .Where(r => r.ParentTagId == request.TagId || r.ChildTagId == request.TagId)
            .Select(r => new TagRelationshipDto
            {
                ParentTagId = r.ParentTagId,
                ChildTagId = r.ChildTagId,
                RelationType = r.RelationType,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return relationships;
    }
}