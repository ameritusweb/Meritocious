using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetSecurityEventsQueryHandler : IRequestHandler<GetSecurityEventsQuery, IEnumerable<SecurityEventDto>>
{
    private readonly MeritociousDbContext context;

    public GetSecurityEventsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<SecurityEventDto>> Handle(GetSecurityEventsQuery request, CancellationToken cancellationToken)
    {
        var query = context.SecurityEvents.AsQueryable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(e => e.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(e => e.Timestamp <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.Severity))
        {
            query = query.Where(e => e.Severity == request.Severity);
        }

        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(e => e.EventType == request.EventType);
        }

        if (request.RequiresAction.HasValue)
        {
            query = query.Where(e => e.RequiresAction == request.RequiresAction.Value);
        }

        var skip = (request.Page - 1) * request.PageSize;

        return await query
            .OrderByDescending(e => e.Timestamp)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(e => new SecurityEventDto
            {
                Id = e.Id,
                EventType = e.EventType,
                Severity = e.Severity,
                UserId = e.UserId,
                UserName = e.UserName,
                Description = e.Description,
                IpAddress = e.IpAddress,
                UserAgent = e.UserAgent,
                Timestamp = e.Timestamp,
                RelatedEntityType = e.RelatedEntityType,
                RelatedEntityId = e.RelatedEntityId,
                RequiresAction = e.RequiresAction,
                Status = e.Status
            })
            .ToListAsync(cancellationToken);
    }
}