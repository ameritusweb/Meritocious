using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Queries;

public record GetSimilarSubstacksQuery(string SubstackId, int Limit = 5)
    : IRequest<List<SubstackDto>>;
