using MediatR;
using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Core.Features.Merit.Queries
{
    public record GetSystemLogsQuery(
        int Page,
        int PageSize,
        string? Level,
        string? SearchText,
        DateTime? StartDate,
        DateTime? EndDate) : IRequest<PagedResult<LogEntryDto>>;

    public record GetRecentLogsQuery(int Count) : IRequest<List<LogEntryDto>>;

    public record GetLogExportUrlQuery(
        string? Level,
        DateTime? StartDate,
        DateTime? EndDate,
        string? SearchText) : IRequest<string>;
}