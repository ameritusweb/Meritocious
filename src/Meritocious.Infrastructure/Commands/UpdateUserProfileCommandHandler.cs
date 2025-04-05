using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Users.Commands;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Results;

namespace Meritocious.Infrastructure.Commands;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly MeritociousDbContext context;

    public UpdateUserProfileCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<UserProfileDto>($"User {request.UserId} not found");
        }

        user.DisplayName = request.Profile.DisplayName ?? user.DisplayName;
        user.Bio = request.Profile.Bio ?? user.Bio;
        user.AvatarUrl = request.Profile.AvatarUrl ?? user.AvatarUrl;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserProfileDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        });
    }
}