using MediatR;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Core.Features.Security.Queries;

public record GetLoginAttemptsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string UserId = null,
    bool? Success = null,
    bool? IsSuspicious = null,
    int Page = 1,
    int PageSize = 20)
    : IRequest<IEnumerable<LoginAttemptDto>>;