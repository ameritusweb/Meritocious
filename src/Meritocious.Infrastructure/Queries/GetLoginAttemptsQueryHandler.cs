using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetLoginAttemptsQueryHandler : IRequestHandler<GetLoginAttemptsQuery, IEnumerable<LoginAttemptDto>>
{
    private readonly MeritociousDbContext context;

    public GetLoginAttemptsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<LoginAttemptDto>> Handle(GetLoginAttemptsQuery request, CancellationToken cancellationToken)
    {
        var query = context.LoginAttempts.AsQueryable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(l => l.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(l => l.Timestamp <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(l => l.UserId.ToString() == request.UserId);
        }

        if (request.Success.HasValue)
        {
            query = query.Where(l => l.Success == request.Success.Value);
        }

        if (request.IsSuspicious.HasValue)
        {
            query = query.Where(l => l.IsSuspicious == request.IsSuspicious.Value);
        }

        var skip = (request.Page - 1) * request.PageSize;

        return await query
            .OrderByDescending(l => l.Timestamp)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(l => new LoginAttemptDto
            {
                Id = l.Id.ToString(),
                UserId = l.UserId == null ? string.Empty : l.UserId.ToString(),
                UserName = l.Username,
                Success = l.Success,
                FailureReason = l.FailureReason,
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                Location = l.Location,
                Timestamp = l.Timestamp,
                AuthMethod = l.AuthMethod,
                IsSuspicious = l.IsSuspicious
            })
            .ToListAsync(cancellationToken);
    }
}