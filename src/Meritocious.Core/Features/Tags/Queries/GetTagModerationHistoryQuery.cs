using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public class GetTagModerationHistoryQuery : IRequest<List<TagModerationLogDto>>
{
    public string TagId { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }

    public GetTagModerationHistoryQuery(string tagId, int page = 1, int pageSize = 20)
    {
        TagId = tagId;
        Page = page;
        PageSize = pageSize;
    }
}