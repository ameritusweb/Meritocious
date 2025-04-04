using MediatR;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Core.Features.Security.Queries;

public record GetApiUsageQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string EndpointPath = null,
    string HttpMethod = null,
    string ClientId = null,
    int Page = 1,
    int PageSize = 20)
    : IRequest<IEnumerable<ApiUsageDto>>;