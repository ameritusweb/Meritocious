using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public class GetTagRelationshipsQuery : IRequest<List<TagRelationshipDto>>
{
    public string TagId { get; private set; }

    public GetTagRelationshipsQuery(string tagId)
    {
        TagId = tagId;
    }
}