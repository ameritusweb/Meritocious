using MediatR;
using Meritocious.Common.DTOs.Moderation;
using Meritocious.Core.Extensions;
using Meritocious.Core.Features.Moderation.Queries;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meritocious.Infrastructure.Queries
{
    public class GetModerationHistoryQueryHandler
    : IRequestHandler<GetModerationHistoryQuery, Result<List<ModerationHistoryDto>>>
    {
        private readonly MeritociousDbContext _context;
        private readonly ILogger<GetModerationHistoryQueryHandler> _logger;

        public GetModerationHistoryQueryHandler(
            MeritociousDbContext context,
            ILogger<GetModerationHistoryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<ModerationHistoryDto>>> Handle(
            GetModerationHistoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get content moderation events
                var moderationEvents = await _context.ContentModerationEvents
                    .Include(e => e.Moderator)
                    .Where(e => e.ContentId == request.ContentId &&
                               e.ContentType == request.ContentType)
                    .OrderByDescending(e => e.ModeratedAt)
                    .ToListAsync(cancellationToken);

                // Get merit score history
                var meritScoreHistory = await _context.MeritScoreHistory
                    .Where(h => h.ContentId == request.ContentId &&
                               h.ContentType == request.ContentType)
                    .OrderByDescending(h => h.UpdatedAt)
                    .ToListAsync(cancellationToken);

                var history = new List<ModerationHistoryDto>();

                foreach (var evt in moderationEvents)
                {
                    // Find the merit scores before and after this moderation event
                    var meritBefore = meritScoreHistory
                        .Where(h => h.UpdatedAt < evt.ModeratedAt)
                        .OrderByDescending(h => h.UpdatedAt)
                        .FirstOrDefault()?.Score;

                    var meritAfter = meritScoreHistory
                        .Where(h => h.UpdatedAt >= evt.ModeratedAt)
                        .OrderBy(h => h.UpdatedAt)
                        .FirstOrDefault()?.Score;

                    history.Add(new ModerationHistoryDto
                    {
                        Timestamp = evt.ModeratedAt,
                        Action = evt.Action.ToDto(),
                        Reason = evt.Reason,
                        IsAutomated = evt.IsAutomated,
                        ModeratorUsername = evt.Moderator?.Username ?? "System",
                        MeritScoreBefore = meritBefore,
                        MeritScoreAfter = meritAfter
                    });
                }

                return Result.Success(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moderation history for content {ContentId}",
                    request.ContentId);
                return Result.Failure<List<ModerationHistoryDto>>(
                    "Error retrieving moderation history");
            }
        }
    }
}
