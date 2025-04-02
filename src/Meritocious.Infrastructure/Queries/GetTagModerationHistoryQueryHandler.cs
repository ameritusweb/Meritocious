using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Queries;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetTagModerationHistoryQueryHandler : IRequestHandler<GetTagModerationHistoryQuery, IEnumerable<TagModerationLogDto>>
{
    private readonly MeritociousDbContext _context;

    public GetTagModerationHistoryQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagModerationLogDto>> Handle(GetTagModerationHistoryQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;

        return await _context.ModerationActions
            .Where(m => m.TagId == request.TagId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(m => new TagModerationLogDto
            {
                Id = m.Id,
                TagId = m.TagId,
                ModeratorId = m.ModeratorId,
                Action = m.Action,
                Reason = m.Reason,
                CreatedAt = m.CreatedAt,
                PreviousState = m.PreviousState,
                NewState = m.NewState
            })
            .ToListAsync(cancellationToken);
    }
}