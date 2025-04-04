using MediatR;
using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Core.Features.Users.Commands;

public record UpdateUserProfileCommand(string UserId, UserProfileDto Profile)
    : IRequest<UserProfileDto>;