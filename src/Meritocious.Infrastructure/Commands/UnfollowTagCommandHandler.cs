using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class UnfollowTagCommandHandler : IRequestHandler<UnfollowTagCommand, bool>
{
    private readonly MeritociousDbContext _context;

    public UnfollowTagCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnfollowTagCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.FollowedTags)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null) return false;

        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);

        if (tag == null) return false;

        if (user.FollowedTags.Remove(tag))
        {
            tag.FollowerCount--;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}