using MediatR;
using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Core.Features.Users.Commands;

public record UpdateUserSettingsCommand(string UserId, UserSettingsDto Settings) : IRequest<bool>;