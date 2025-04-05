using MediatR;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Core.Results;

namespace Meritocious.Core.Features.Users.Commands;

public class UpdateUserProfileCommand : IRequest<Result<UserProfileDto>>
{
    public string UserId { get; set; }
    public UserProfileUpdateDto Profile { get; set; }
}