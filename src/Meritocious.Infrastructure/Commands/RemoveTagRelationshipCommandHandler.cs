using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class RemoveTagRelationshipCommandHandler : IRequestHandler<RemoveTagRelationshipCommand, bool>
{
    private readonly MeritociousDbContext context;

    public RemoveTagRelationshipCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<bool> Handle(RemoveTagRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationship = await context.TagRelationships
            .FirstOrDefaultAsync(r => 
                r.ParentTagId == request.ParentTagId && 
                r.ChildTagId == request.ChildTagId, 
                cancellationToken);

        if (relationship == null)
        {
            return false;
        }

        context.TagRelationships.Remove(relationship);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}