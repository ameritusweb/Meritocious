using Meritocious.Common.Enums;
using Meritocious.Core.Features.Reporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    public interface IReportingService
    {
        Task<ContentReport> CreateReportAsync(
            Guid contentId,
            ContentType contentType,
            Guid reporterId,
            string reportType,
            string description);

        Task<ContentReport> GetReportByIdAsync(Guid reportId);

        Task<List<ContentReport>> GetReportsAsync(
            string status = "pending",
            string sortBy = "date",
            int? page = null,
            int? pageSize = null);

        Task<ContentReport> ResolveReportAsync(
            Guid reportId,
            Guid moderatorId,
            string resolution,
            string notes);

        Task<bool> ShouldTriggerModerationAsync(ContentReport report);
    }
}
