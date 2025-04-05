using Meritocious.Common.Enums;
using Meritocious.Core.Entities;

namespace Meritocious.Core.Interfaces
{
    public interface IReportingService
    {
        Task<ContentReport> CreateReportAsync(
            string contentId,
            ContentType contentType,
            string reporterId,
            string reportType,
            string description);

        Task<ContentReport> GetReportByIdAsync(string reportId);

        Task<List<ContentReport>> GetReportsAsync(
            string status = "pending",
            string sortBy = "date",
            int? page = null,
            int? pageSize = null);

        Task<ContentReport> ResolveReportAsync(
            string reportId,
            string moderatorId,
            string resolution,
            string notes);

        Task<bool> ShouldTriggerModerationAsync(ContentReport report);
    }
}
