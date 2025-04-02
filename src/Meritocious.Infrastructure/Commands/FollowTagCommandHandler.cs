using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Commands;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class FollowTagCommandHandler : IRequestHandler<FollowTagCommand, bool>
{
    private readonly MeritociousDbContext _context;

    public FollowTagCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(FollowTagCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.FollowedTags)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null) return false;

        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);

        if (tag == null) return false;

        if (!user.FollowedTags.Contains(tag))
        {
            user.FollowedTags.Add(tag);
            tag.FollowerCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}