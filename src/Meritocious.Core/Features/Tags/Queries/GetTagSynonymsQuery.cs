using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public record GetTagSynonymsQuery(string TagId) : IRequest<IEnumerable<TagSynonymDto>>;