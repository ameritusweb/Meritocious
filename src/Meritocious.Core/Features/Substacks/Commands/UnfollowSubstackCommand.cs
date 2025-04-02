using MediatR;

namespace Meritocious.Core.Features.Substacks.Commands;

public record UnfollowSubstackCommand(string UserId, string SubstackId) : IRequest<bool>;
