using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Substacks.Commands;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Exceptions;

namespace Meritocious.Infrastructure.Commands;

public class FollowSubstackCommandHandler : IRequestHandler<FollowSubstackCommand, bool>
{
    private readonly MeritociousDbContext context;

    public FollowSubstackCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<bool> Handle(FollowSubstackCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.FollowedSubstacks)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new ResourceNotFoundException($"User with ID {request.UserId} not found");
        }

        var substack = await context.Substacks
            .FirstOrDefaultAsync(s => s.Id.ToString() == request.SubstackId, cancellationToken);

        if (substack == null)
        {
            throw new ResourceNotFoundException($"Substack with ID {request.SubstackId} not found");
        }

        if (user.FollowedSubstacks.Any(s => s.Id.ToString() == request.SubstackId))
        {
            return false;
        }

        user.FollowedSubstacks.Add(substack);
        substack.FollowerCount++;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}