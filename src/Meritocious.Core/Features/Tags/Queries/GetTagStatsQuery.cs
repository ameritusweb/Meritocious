using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public record GetTagStatsQuery(string TagId)
    : IRequest<TagDto>;