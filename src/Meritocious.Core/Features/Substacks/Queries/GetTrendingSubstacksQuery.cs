using MediatR;
using Meritocious.Common.DTOs.Substacks;
using Meritocious.Core.Results;

namespace Meritocious.Core.Features.Substacks.Queries;

public record GetTrendingSubstacksQuery(int Limit = 10, int Skip = 0)
    : IRequest<Result<List<SubstackDto>>>;
