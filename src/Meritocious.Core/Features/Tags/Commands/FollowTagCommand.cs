using MediatR;

namespace Meritocious.Core.Features.Tags.Commands;

public class FollowTagCommand : IRequest<bool>
{
    public string UserId { get; private set; }
    public string TagId { get; private set; }

    public FollowTagCommand(string userId, string tagId)
    {
        UserId = userId;
        TagId = tagId;
    }
}