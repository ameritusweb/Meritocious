using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public class GetTagStatsQuery : IRequest<TagDto>
{
    public string TagId { get; private set; }

    public GetTagStatsQuery(string tagId)
    {
        TagId = tagId;
    }
}