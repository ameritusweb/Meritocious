using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries
{
    public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, AdminStats>
    {
        private readonly MeritociousDbContext _context;

        public GetAdminStatsQueryHandler(MeritociousDbContext context)
        {
            _context = context;
        }

        public async Task<AdminStats> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var today = DateTime.UtcNow.Date;

            return new AdminStats
            {
                TotalUsers = await _context.Users.CountAsync(cancellationToken),
                ActiveUsers24h = await _context.Users
                    .CountAsync(u => u.LastActiveAt >= yesterday, cancellationToken),
                PostsToday = await _context.Posts
                    .CountAsync(p => p.CreatedAt >= today, cancellationToken)
            };
        }
    }
}