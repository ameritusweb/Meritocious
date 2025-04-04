using MediatR;
using Meritocious.Common.DTOs.Content;

namespace Meritocious.Core.Features.Posts.Queries
{
    public record GetPostHistoryQuery(
        Guid PostId,
        int? StartVersion = null,
        int? EndVersion = null,
        bool IncludeContent = true)
        : IRequest<List<PostVersionDto>>;
}