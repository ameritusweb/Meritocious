using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public record GetTagModerationHistoryQuery(string TagId, int Page = 1, int PageSize = 20) : IRequest<IEnumerable<TagModerationLogDto>>;