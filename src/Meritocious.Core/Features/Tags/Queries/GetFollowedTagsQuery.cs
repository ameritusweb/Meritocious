using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public class GetFollowedTagsQuery : IRequest<List<TagDto>>
{
    public string UserId { get; private set; }

    public GetFollowedTagsQuery(string userId)
    {
        UserId = userId;
    }
}