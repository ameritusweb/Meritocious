using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Queries;

public record GetRecommendedSubstacksQuery(string UserId, int Limit = 10) : IRequest<List<SubstackDto>>;
