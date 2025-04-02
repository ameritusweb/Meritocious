using MediatR;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Core.Features.Security.Queries;

public record GetAdminActionsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string AdminId = null,
    string ActionType = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<IEnumerable<AdminActionDto>>;