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
            query = query.Where(a => a.EndpointPath == request.EndpointPath);
        }

        if (!string.IsNullOrEmpty(request.HttpMethod))
        {
            query = query.Where(a => a.HttpMethod == request.HttpMethod);
        }

        if (!string.IsNullOrEmpty(request.ClientId))
        {
            query = query.Where(a => a.ClientId == request.ClientId);
        }

        var skip = (request.Page - 1) * request.PageSize;

        return await query
            .OrderByDescending(a => a.TimeStamp)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(a => new ApiUsageDto
            {
                EndpointPath = a.EndpointPath,
                HttpMethod = a.HttpMethod,
                TotalRequests = a.TotalRequests,
                SuccessfulRequests = a.SuccessfulRequests,
                FailedRequests = a.FailedRequests,
                AverageResponseTime = a.AverageResponseTime,
                ErrorRate = a.ErrorRate,
                ResponseStatusCodes = a.ResponseStatusCodes,
                ErrorTypes = a.ErrorTypes,
                TimeStamp = a.TimeStamp,
                ClientId = a.ClientId
            })
            .ToListAsync(cancellationToken);
    }
}