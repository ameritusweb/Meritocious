using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Queries;

public record GetTrendingSubstacksQuery(int Limit = 10, int Skip = 0) : IRequest<List<SubstackDto>>;
