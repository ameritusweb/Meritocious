using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public record GetFollowedTagsQuery(string UserId)
    : IRequest<IEnumerable<TagDto>>;