using MediatR;
using Meritocious.Common.DTOs;
using Meritocious.Common.DTOs.Contributions;

namespace Meritocious.Core.Features.Users.Queries;

public record GetUserContributionsQuery(string UserId, int Page = 1, int PageSize = 20)
    : IRequest<IEnumerable<ContributionSummaryDto>>;