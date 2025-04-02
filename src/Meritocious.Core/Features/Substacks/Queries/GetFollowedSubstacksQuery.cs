using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Queries;

public record GetFollowedSubstacksQuery(string UserId, int Limit = 20, int Skip = 0) : IRequest<List<SubstackDto>>;
