using MediatR;

namespace Meritocious.Core.Features.Security.Queries;

public record GetAuditLogExportQuery(
    DateTime StartDate,
    DateTime EndDate,
    string ExportFormat = "csv",
    bool IncludeAdminActions = true,
    bool IncludeSecurityEvents = true,
    bool IncludeLoginAttempts = true,
    bool IncludeApiUsage = true
) : IRequest<byte[]>;