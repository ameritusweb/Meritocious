using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetAdminActionsQueryHandler : IRequestHandler<GetAdminActionsQuery, IEnumerable<AdminActionDto>>
{
    private readonly MeritociousDbContext context;

    public GetAdminActionsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<AdminActionDto>> Handle(GetAdminActionsQuery request, CancellationToken cancellationToken)
    {
        var query = context.AdminActions.AsQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(a => a.Timestamp >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(a => a.Timestamp <= request.EndDate.Value);

        if (!string.IsNullOrEmpty(request.AdminId))
            query = query.Where(a => a.AdminId == request.AdminId);

        if (!string.IsNullOrEmpty(request.ActionType))
            query = query.Where(a => a.ActionType == request.ActionType);

        var skip = (request.Page - 1) * request.PageSize;

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(a => new AdminActionDto
            {
                Id = a.Id,
                AdminId = a.AdminId,
                AdminName = a.AdminName,
                ActionType = a.ActionType,
                TargetType = a.TargetType,
                TargetId = a.TargetId,
                Details = a.Details,
                Reason = a.Reason,
                Timestamp = a.Timestamp,
                IpAddress = a.IpAddress,
                Status = a.Status
            })
            .ToListAsync(cancellationToken);
    }
}