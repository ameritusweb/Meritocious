using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Users.Commands;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Commands;

public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, bool>
{
    private readonly MeritociousDbContext context;

    public UpdateUserSettingsCommandHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<bool> Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return false;
        }

        user.DisplayName = request.Settings.DisplayName ?? user.DisplayName;
        user.Bio = request.Settings.Bio ?? user.Bio;
        user.AvatarUrl = request.Settings.AvatarUrl ?? user.AvatarUrl;
        user.EmailNotificationsEnabled = request.Settings.EmailNotificationsEnabled;
        user.PublicProfile = request.Settings.PublicProfile;
        user.PreferredTags = request.Settings.PreferredTags?.ToList() ?? user.PreferredTags;
        user.TimeZone = request.Settings.TimeZone ?? user.TimeZone;
        user.Language = request.Settings.Language ?? user.Language;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}