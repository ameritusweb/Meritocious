using MediatR;
using Meritocious.Common.DTOs.Moderation;
using Meritocious.Common.Enums;
using Meritocious.Core.Results;

namespace Meritocious.Core.Features.Moderation.Queries
{
    public record GetModerationHistoryQuery : IRequest<Result<List<ModerationHistoryDto>>>
    {
        public string ContentId { get; init; }
        public ContentType ContentType { get; init; }
    }
}
