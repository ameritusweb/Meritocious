using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Users.Commands;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    private readonly MeritociousDbContext _context;

    public UpdateUserProfileCommandHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null) throw new KeyNotFoundException($"User {request.UserId} not found");

        user.DisplayName = request.Profile.DisplayName ?? user.DisplayName;
        user.Bio = request.Profile.Bio ?? user.Bio;
        user.AvatarUrl = request.Profile.AvatarUrl ?? user.AvatarUrl;

        await _context.SaveChangesAsync(cancellationToken);

        return new UserProfileDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        };
    }
}