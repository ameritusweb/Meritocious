using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public class GetUserTagsQuery : IRequest<List<TagDto>>
{
    public string UserId { get; private set; }

    public GetUserTagsQuery(string userId)
    {
        UserId = userId;
    }
}