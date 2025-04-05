namespace Meritocious.Common.DTOs.Auth
{
    public class LogEntryDto
    {
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Exception { get; set; }
        public string? AdditionalData { get; set; }
    }

    public class LogQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Level { get; set; }
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AdminDashboardStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveModerations { get; set; }
        public int PostsToday { get; set; }
        public int ActiveUsers24h { get; set; }
    }
}