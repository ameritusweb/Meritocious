using MediatR;

namespace Meritocious.Core.Features.Tags.Commands;

public record RemoveTagRelationshipCommand(string ParentTagId, string ChildTagId)
    : IRequest<bool>;