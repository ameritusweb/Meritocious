using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Features.Tags.Models;

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
        var sourceTag = await context.Tags.FirstOrDefaultAsync(x => x.Id.ToString() == request.ParentTagId);
        var relatedTag = await context.Tags.FirstOrDefaultAsync(x => x.Id.ToString() == request.ChildTagId);
        var creator = await context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == request.CreatedBy);
        var relationship = Core.Entities.TagRelationship.Create(
            sourceTag,
            relatedTag,
            (TagRelationType)request.RelationType,
            0m, // TODO: Get strength
            creator);

        context.TagRelationships.Add(relationship);
        await context.SaveChangesAsync(cancellationToken);

        return new TagRelationshipDto
        {
            ParentTagId = relationship.ParentTagId,
            ChildTagId = relationship.ChildTagId,
            RelationType = relationship.RelationType.ToString(),
            CreatedBy = relationship.Creator.Id.ToString(),
            CreatedAt = relationship.CreatedAt
        };
    }
}