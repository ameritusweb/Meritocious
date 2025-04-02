using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Queries;

public record GetUserTagsQuery(string UserId) : IRequest<IEnumerable<TagDto>>;