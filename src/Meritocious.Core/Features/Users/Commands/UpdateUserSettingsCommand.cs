using MediatR;
using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Core.Features.Users.Commands;

public class UpdateUserSettingsCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public UserSettingsDto Settings { get; set; }
}