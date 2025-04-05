using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public class GetTagSynonymsQuery : IRequest<List<TagSynonymDto>>
{
    public string TagId { get; private set; }

    public GetTagSynonymsQuery(string tagId)
    {
        TagId = tagId;
    }
}