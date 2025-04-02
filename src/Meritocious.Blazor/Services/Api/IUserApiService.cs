using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Blazor.Services.Api
{
    public interface IUserApiService
    {
        Task<AdminDashboardStatistics> GetAdminDashboardStatisticsAsync();
        Task<PagedResult<LogEntryDto>> GetSystemLogsAsync(LogQueryParams queryParams);
        Task<List<LogEntryDto>> GetRecentSystemLogsAsync(int count);
        Task<string> GetLogExportUrlAsync(string? level, DateTime? startDate, DateTime? endDate, string? searchText);
    }
}