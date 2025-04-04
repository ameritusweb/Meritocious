using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public record GetTagRelationshipsQuery(string TagId)
    : IRequest<IEnumerable<TagRelationshipDto>>;