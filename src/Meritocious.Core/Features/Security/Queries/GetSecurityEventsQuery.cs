using MediatR;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Core.Features.Security.Queries;

public record GetSecurityEventsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string Severity = null,
    string EventType = null,
    bool? RequiresAction = null,
    int Page = 1,
    int PageSize = 20)
    : IRequest<IEnumerable<SecurityEventDto>>;