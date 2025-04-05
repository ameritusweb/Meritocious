using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetApiUsageQueryHandler : IRequestHandler<GetApiUsageQuery, IEnumerable<ApiUsageDto>>
{
    private readonly MeritociousDbContext context;

    public GetApiUsageQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<ApiUsageDto>> Handle(GetApiUsageQuery request, CancellationToken cancellationToken)
    {
        var query = context.ApiUsageLogs.AsQueryable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.EndpointPath))
        {
            query = query.Where(a => a.Endpoint == request.EndpointPath);
        }

        if (!string.IsNullOrEmpty(request.HttpMethod))
        {
            query = query.Where(a => a.Method == request.HttpMethod);
        }

        if (!string.IsNullOrEmpty(request.ClientId))
        {
            query = query.Where(a => a.UserId.ToString() == request.ClientId);
        }

        // Group + aggregate
        var groupedQuery = query
            .GroupBy(a => new { a.Endpoint, a.Method })
            .Select(g => new ApiUsageDto
            {
                EndpointPath = g.Key.Endpoint,
                HttpMethod = g.Key.Method,
                TotalRequests = g.Count(),
                SuccessfulRequests = g.Count(x => x.StatusCode >= 200 && x.StatusCode < 300),
                FailedRequests = g.Count(x => x.StatusCode >= 400),
                AverageResponseTime = g.Average(x => x.DurationMs),
                ErrorRate = g.Count(x => x.StatusCode >= 400) * 1.0 / g.Count(),
                TimeStamp = g.Max(x => x.Timestamp),
                ClientId = null // Optional if grouping by endpoint
            });

        // Apply paging on aggregated results
        var skip = (request.Page - 1) * request.PageSize;
        var pagedResult = await groupedQuery
            .OrderByDescending(a => a.TotalRequests)
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return pagedResult;
    }
}