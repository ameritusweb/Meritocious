using MediatR;
using Meritocious.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public class GetModerationStatsQueryHandler : IRequestHandler<GetModerationStatsQuery, ModerationStatsDto>
    {
        private readonly MeritociousDbContext _context;
        private readonly ILogger<GetModerationStatsQueryHandler> _logger;

        public GetModerationStatsQueryHandler(
            MeritociousDbContext context,
            ILogger<GetModerationStatsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ModerationStatsDto> Handle(
            GetModerationStatsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = request.TimeFrame switch
                {
                    "hour" => DateTime.UtcNow.AddHours(-1),
                    "day" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    _ => DateTime.UtcNow.AddDays(-1)
                };

                var reports = await _context.ContentReports
                    .Where(r => r.CreatedAt >= startDate)
                    .ToListAsync(cancellationToken);

                var pendingReports = reports.Count(r => r.Status == "pending");
                var resolvedReports = reports.Count(r => r.Status == "resolved");

                var reportsByType = reports
                    .GroupBy(r => r.ReportType)
                    .ToDictionary(g => g.Key, g => g.Count());

                var actionsTaken = reports
                    .Where(r => r.Status == "resolved")
                    .GroupBy(r => r.Resolution)
                    .ToDictionary(g => g.Key, g => g.Count());

                var averageResolutionTime = reports
                    .Where(r => r.Status == "resolved" && r.ResolvedAt.HasValue)
                    .Select(r => (r.ResolvedAt.Value - r.CreatedAt).TotalHours)
                    .DefaultIfEmpty(0)
                    .Average();

                return new ModerationStatsDto
                {
                    TotalReports = reports.Count,
                    PendingReports = pendingReports,
                    ResolvedReports = resolvedReports,
                    ReportsByType = reportsByType,
                    ActionsTaken = actionsTaken,
                    AverageResolutionTime = (decimal)averageResolutionTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moderation stats");
                throw;
            }
        }
    }
