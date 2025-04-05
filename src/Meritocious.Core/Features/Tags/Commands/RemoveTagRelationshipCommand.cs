using MediatR;

namespace Meritocious.Core.Features.Tags.Commands;

public class RemoveTagRelationshipCommand : IRequest<bool>
{
    public string ParentTagId { get; private set; }
    public string ChildTagId { get; private set; }

    public RemoveTagRelationshipCommand(string parentTagId, string childTagId)
    {
        ParentTagId = parentTagId;
        ChildTagId = childTagId;
    }
}