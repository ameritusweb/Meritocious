using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class AddTagSynonymCommandHandler : IRequestHandler<AddTagSynonymCommand, TagSynonymDto>
{
    private readonly MeritociousDbContext context;

    public AddTagSynonymCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<TagSynonymDto> Handle(AddTagSynonymCommand request, CancellationToken cancellationToken)
    {
        var synonym = new Core.Entities.TagSynonym
        {
            SourceTagId = request.SourceTagId,
            TargetTagId = request.TargetTagId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        context.TagSynonyms.Add(synonym);
        await context.SaveChangesAsync(cancellationToken);

        return new TagSynonymDto
        {
            SourceTagId = synonym.SourceTagId,
            TargetTagId = synonym.TargetTagId,
            CreatedBy = synonym.CreatedBy,
            CreatedAt = synonym.CreatedAt,
            Status = synonym.Status
        };
    }
}