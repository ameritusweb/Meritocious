using MediatR;

namespace Meritocious.Core.Features.Tags.Commands;

public record UnfollowTagCommand(string UserId, string TagId)
    : IRequest<bool>;