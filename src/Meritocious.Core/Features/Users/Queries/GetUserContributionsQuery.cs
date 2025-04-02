using MediatR;
using Meritocious.Common.DTOs;

namespace Meritocious.Core.Features.Users.Queries;

public record GetUserContributionsQuery(string UserId, int Page = 1, int PageSize = 20) : IRequest<IEnumerable<ContributionSummaryDto>>;