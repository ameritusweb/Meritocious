using Meritocious.Common.Enums;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meritocious.Infrastructure.Data.Services
{
    public class ReportingService : IReportingService
    {
        private readonly MeritociousDbContext context;
        private readonly ILogger<ReportingService> logger;

        public ReportingService(
            MeritociousDbContext context,
            ILogger<ReportingService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<ContentReport> CreateReportAsync(
            string contentId,
            ContentType contentType,
            string reporterId,
            string reportType,
            string description)
        {
            try
            {
                // Check if content exists
                bool contentExists = contentType switch
                {
                    ContentType.Post => await context.Posts.AnyAsync(p => p.Id == contentId),
                    ContentType.Comment => await context.Comments.AnyAsync(c => c.Id == contentId),
                    _ => false
                };

                if (!contentExists)
                {
                    throw new KeyNotFoundException($"{contentType} with ID {contentId} not found");
                }

                // Create report
                var report = ContentReport.Create(
                    contentId,
                    contentType,
                    reporterId,
                    reportType,
                    description);

                await context.ContentReports.AddAsync(report);
                await context.SaveChangesAsync();

                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating report for {ContentType} {ContentId}", contentType, contentId);
                throw;
            }
        }

        public async Task<ContentReport> GetReportByIdAsync(string reportId)
        {
            try
            {
                var report = await context.ContentReports
                    .SingleOrDefaultAsync(r => r.Id == reportId);

                if (report == null)
                {
                    throw new KeyNotFoundException($"Report with ID {reportId} not found");
                }

                return report;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                logger.LogError(ex, "Error getting report {ReportId}", reportId);
                throw;
            }
        }

        public async Task<List<ContentReport>> GetReportsAsync(
            string status = "pending",
            string sortBy = "date",
            int? page = null,
            int? pageSize = null)
        {
            try
            {
                var query = context.ContentReports.AsQueryable();

                // Filter by status
                if (status != "all")
                {
                    query = query.Where(r => r.Status == status);
                }

                // Apply sorting
                query = sortBy switch
                {
                    "date" => query.OrderByDescending(r => r.CreatedAt),
                    "severity" => query.OrderByDescending(r => r.ReportType),
                    "type" => query.OrderBy(r => r.ContentType).ThenByDescending(r => r.CreatedAt),
                    _ => query.OrderByDescending(r => r.CreatedAt)
                };

                // Apply pagination
                if (page.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting reports");
                throw;
            }
        }

        public async Task<ContentReport> ResolveReportAsync(
            string reportId,
            string moderatorId,
            string resolution,
            string notes)
        {
            try
            {
                var report = await GetReportByIdAsync(reportId);

                report.Resolve(moderatorId, resolution, notes);
                await context.SaveChangesAsync();

                return report;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                logger.LogError(ex, "Error resolving report {ReportId}", reportId);
                throw;
            }
        }

        public async Task<bool> ShouldTriggerModerationAsync(ContentReport report)
        {
            // Decide if this report should trigger immediate moderation
            // For example, based on severity, reporter reputation, etc.

            // In a real implementation, this would be more sophisticated
            return report.ReportType.ToLowerInvariant() switch
            {
                "spam" => true,
                "harmful" => true,
                "severe" => true,
                _ => false
            };
        }
    }
}
