using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Substacks.Commands;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Exceptions;

namespace Meritocious.Infrastructure.Commands;

public class UnfollowSubstackCommandHandler : IRequestHandler<UnfollowSubstackCommand, bool>
{
    private readonly MeritociousDbContext _context;

    public UnfollowSubstackCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnfollowSubstackCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.FollowedSubstacks)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new ResourceNotFoundException($"User with ID {request.UserId} not found");
        }

        var substack = await _context.Substacks
            .FirstOrDefaultAsync(s => s.Id == request.SubstackId, cancellationToken);

        if (substack == null)
        {
            throw new ResourceNotFoundException($"Substack with ID {request.SubstackId} not found");
        }

        var removed = user.FollowedSubstacks.Remove(substack);
        if (removed)
        {
            substack.FollowerCount--;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return removed;
    }
}