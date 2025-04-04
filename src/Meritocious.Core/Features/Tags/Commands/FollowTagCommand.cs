using MediatR;

namespace Meritocious.Core.Features.Tags.Commands;

public record FollowTagCommand(string UserId, string TagId)
    : IRequest<bool>;