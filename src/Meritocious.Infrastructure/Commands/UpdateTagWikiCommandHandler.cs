using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class UpdateTagWikiCommandHandler : IRequestHandler<UpdateTagWikiCommand, TagWikiDto>
{
    private readonly MeritociousDbContext _context;

    public UpdateTagWikiCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<TagWikiDto> Handle(UpdateTagWikiCommand request, CancellationToken cancellationToken)
    {
        var tagWiki = await _context.TagWikis
            .FirstOrDefaultAsync(w => w.TagId == request.TagId, cancellationToken);

        var revisionNumber = 1;
        if (tagWiki != null)
        {
            revisionNumber = tagWiki.RevisionNumber + 1;
        }
        else
        {
            tagWiki = new Core.Entities.TagWiki
            {
                TagId = request.TagId
            };
            _context.TagWikis.Add(tagWiki);
        }

        tagWiki.Content = request.Content;
        tagWiki.LastEditedBy = request.EditedBy;
        tagWiki.LastEditedAt = DateTime.UtcNow;
        tagWiki.EditSummary = request.EditSummary;
        tagWiki.RevisionNumber = revisionNumber;

        await _context.SaveChangesAsync(cancellationToken);

        return new TagWikiDto
        {
            TagId = tagWiki.TagId,
            Content = tagWiki.Content,
            LastEditedBy = tagWiki.LastEditedBy,
            LastEditedAt = tagWiki.LastEditedAt,
            EditSummary = tagWiki.EditSummary,
            RevisionNumber = tagWiki.RevisionNumber
        };
    }
}