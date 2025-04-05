using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class FollowTagCommandHandler : IRequestHandler<FollowTagCommand, bool>
{
    private readonly MeritociousDbContext context;

    public FollowTagCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<bool> Handle(FollowTagCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.FollowedTags)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return false;
        }

        var tag = await context.Tags
            .FirstOrDefaultAsync(t => t.Id.ToString() == request.TagId, cancellationToken);

        if (tag == null)
        {
            return false;
        }

        if (!user.FollowedTags.Contains(tag))
        {
            user.FollowedTags.Add(tag);
            tag.FollowerCount++;
            await context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}