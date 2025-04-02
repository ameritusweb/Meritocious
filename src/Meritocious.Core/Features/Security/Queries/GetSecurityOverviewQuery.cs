using MediatR;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Core.Features.Security.Queries;

public record GetSecurityOverviewQuery : IRequest<SecurityOverviewDto>;