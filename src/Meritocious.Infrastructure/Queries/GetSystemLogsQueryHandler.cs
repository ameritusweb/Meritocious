using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Results;
using Microsoft.Extensions.Configuration;

namespace Meritocious.Infrastructure.Queries
{
    public class GetSystemLogsQueryHandler : 
        IRequestHandler<GetSystemLogsQuery, PagedResult<LogEntryDto>>,
        IRequestHandler<GetRecentLogsQuery, List<LogEntryDto>>,
        IRequestHandler<GetLogExportUrlQuery, string>
    {
        private readonly MeritociousDbContext context;
        private readonly IConfiguration configuration;

        public GetSystemLogsQueryHandler(
            MeritociousDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<PagedResult<LogEntryDto>> Handle(
            GetSystemLogsQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.SecurityAuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(request.Level))
            {
                query = query.Where(l => l.Level == request.Level);
            }

            if (!string.IsNullOrEmpty(request.SearchText))
                query = query.Where(l => l.Message.Contains(request.SearchText) ||
                                       l.Source.Contains(request.SearchText));

            if (request.StartDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= request.EndDate.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new LogEntryDto
                {
                    Level = l.Level,
                    Message = l.Message,
                    Source = l.Source,
                    Timestamp = l.Timestamp,
                    Exception = l.Exception,
                    AdditionalData = l.AdditionalData
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<LogEntryDto>
            {
                Items = logs,
                TotalCount = totalItems,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<List<LogEntryDto>> Handle(
            GetRecentLogsQuery request,
            CancellationToken cancellationToken)
        {
            return await context.SecurityAuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(request.Count)
                .Select(l => new LogEntryDto
                {
                    Level = l.Level,
                    Message = l.Message,
                    Source = l.Source,
                    Timestamp = l.Timestamp,
                    Exception = l.Exception,
                    AdditionalData = l.AdditionalData
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<string> Handle(
            GetLogExportUrlQuery request,
            CancellationToken cancellationToken)
        {
            // Generate a temporary signed URL for downloading logs
            var baseUrl = configuration["StorageBaseUrl"];
            var fileName = $"logs-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
            
            // TODO: Implement actual log export generation and URL signing
            return $"{baseUrl}/exports/{fileName}";
        }
    }
}