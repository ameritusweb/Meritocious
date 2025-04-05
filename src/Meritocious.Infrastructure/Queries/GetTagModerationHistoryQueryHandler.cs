using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetTagModerationHistoryQueryHandler : IRequestHandler<GetTagModerationHistoryQuery, IEnumerable<TagModerationLogDto>>
{
    private readonly MeritociousDbContext context;

    public GetTagModerationHistoryQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TagModerationLogDto>> Handle(GetTagModerationHistoryQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;

        return await context.ModerationActions
            .Where(m => m.TagId.ToString() == request.TagId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(m => new TagModerationLogDto
            {
                Id = m.Id.ToString(),
                TagId = m.TagId.ToString(),
                ModeratorId = m.ModeratorId.ToString(),
                Action = m.Action,
                Reason = m.Reason,
                CreatedAt = m.CreatedAt,
                PreviousState = m.PreviousState,
                NewState = m.NewState
            })
            .ToListAsync(cancellationToken);
    }
}