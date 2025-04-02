using MediatR;

namespace Meritocious.Core.Features.Substacks.Commands;

public record FollowSubstackCommand(string UserId, string SubstackId) : IRequest<bool>;
