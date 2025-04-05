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
        var query = context.AdminActionLogs.AsQueryable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.AdminId))
        {
            query = query.Where(a => a.AdminUserId == request.AdminId);
        }

        if (!string.IsNullOrEmpty(request.ActionType))
        {
            query = query.Where(a => a.Action == request.ActionType);
        }

        var skip = (request.Page - 1) * request.PageSize;

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(a => new AdminActionDto
            {
                Id = a.Id.ToString(),
                AdminId = a.AdminUserId.ToString(),
                AdminName = a.AdminUser.UserName, // if available
                ActionType = a.Action,
                TargetType = a.Metadata.ContainsKey("TargetType") ? a.Metadata["TargetType"].ToString() : null,
                TargetId = a.Metadata.ContainsKey("TargetId") ? a.Metadata["TargetId"].ToString() : null,
                Details = a.Details,
                Reason = a.Metadata.ContainsKey("Reason") ? a.Metadata["Reason"].ToString() : null,
                Timestamp = a.Timestamp,
                IpAddress = a.IpAddress,
                Status = a.Metadata.ContainsKey("Status") ? a.Metadata["Status"].ToString() : null
            })
            .ToListAsync(cancellationToken);
    }
}