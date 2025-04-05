using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries
{
    public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, AdminStats>
    {
        private readonly MeritociousDbContext context;

        public GetAdminStatsQueryHandler(MeritociousDbContext context)
        {
            this.context = context;
        }

        public async Task<AdminStats> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var today = DateTime.UtcNow.Date;

            return new AdminStats
            {
                TotalUsers = await context.Users.CountAsync(cancellationToken),
                ActiveUsers24h = await context.Users
                    .CountAsync(u => u.LastActiveAt >= yesterday, cancellationToken),
                PostsToday = await context.Posts
                    .CountAsync(p => p.CreatedAt >= today, cancellationToken)
            };
        }
    }
}