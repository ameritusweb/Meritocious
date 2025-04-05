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
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.CreatedBy);
        var synonym = Core.Entities.TagSynonym.Create(
            Guid.Parse(request.SourceTagId),
            Guid.Parse(request.TargetTagId),
            user);

        context.TagSynonyms.Add(synonym);
        await context.SaveChangesAsync(cancellationToken);

        return new TagSynonymDto
        {
            SourceTagId = synonym.SourceTagId.ToString(),
            TargetTagId = synonym.TargetTagId.ToString(),
            CreatedBy = synonym.CreatedBy.Id.ToString(),
            CreatedAt = synonym.CreatedAt,
            Status = synonym.Status
        };
    }
}