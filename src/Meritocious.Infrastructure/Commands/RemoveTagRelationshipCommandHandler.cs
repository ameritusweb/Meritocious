using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class RemoveTagRelationshipCommandHandler : IRequestHandler<RemoveTagRelationshipCommand, bool>
{
    private readonly MeritociousDbContext _context;

    public RemoveTagRelationshipCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveTagRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _context.TagRelationships
            .FirstOrDefaultAsync(r => 
                r.ParentTagId == request.ParentTagId && 
                r.ChildTagId == request.ChildTagId, 
                cancellationToken);

        if (relationship == null) return false;

        _context.TagRelationships.Remove(relationship);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}