using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class CreateTagRelationshipCommandHandler : IRequestHandler<CreateTagRelationshipCommand, TagRelationshipDto>
{
    private readonly MeritociousDbContext context;

    public CreateTagRelationshipCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<TagRelationshipDto> Handle(CreateTagRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationship = new Core.Entities.TagRelationship
        {
            ParentTagId = request.ParentTagId,
            ChildTagId = request.ChildTagId,
            RelationType = request.RelationType,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        context.TagRelationships.Add(relationship);
        await context.SaveChangesAsync(cancellationToken);

        return new TagRelationshipDto
        {
            ParentTagId = relationship.ParentTagId,
            ChildTagId = relationship.ChildTagId,
            RelationType = relationship.RelationType,
            CreatedBy = relationship.CreatedBy,
            CreatedAt = relationship.CreatedAt
        };
    }
}