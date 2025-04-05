using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Commands;

public class UpdateTagWikiCommandHandler : IRequestHandler<UpdateTagWikiCommand, TagWikiDto>
{
    private readonly MeritociousDbContext context;

    public UpdateTagWikiCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<TagWikiDto> Handle(UpdateTagWikiCommand request, CancellationToken cancellationToken)
    {
        var tag = await context.Tags
        .Include(t => t.WikiVersions)
        .FirstOrDefaultAsync(t => t.Id.ToString() == request.TagId, cancellationToken);

        var editor = await context.Users.FindAsync(Guid.Parse(request.EditorId));

        var newWiki = TagWiki.Create(tag, request.Content, editor, request.EditReason);
        context.TagWikis.Add(newWiki);

        await context.SaveChangesAsync(cancellationToken);

        return new TagWikiDto
        {
            TagId = newWiki.TagId.ToString(),
            Content = newWiki.Content,
            LastEditedBy = editor.DisplayName, // assuming that’s a field
            LastEditedAt = newWiki.CreatedAt,
            EditSummary = newWiki.EditReason,
            RevisionNumber = newWiki.VersionNumber
        };
    }
}