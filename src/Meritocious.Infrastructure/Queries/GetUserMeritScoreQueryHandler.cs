using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Common.DTOs.Merit;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetUserMeritScoreQueryHandler : IRequestHandler<GetUserMeritScoreQuery, MeritScoreDto>
{
    private readonly MeritociousDbContext _context;

    public GetUserMeritScoreQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<MeritScoreDto> Handle(GetUserMeritScoreQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.MeritScoreHistory)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null) throw new KeyNotFoundException($"User {request.UserId} not found");

        return new MeritScoreDto
        {
            UserId = user.Id,
            CurrentScore = user.CurrentMeritScore,
            LastCalculated = user.LastMeritScoreUpdate,
            ScoreHistory = user.MeritScoreHistory.Select(h => new MeritScoreHistoryDto
            {
                Score = h.Score,
                Timestamp = h.Timestamp,
                Reason = h.Reason
            }).ToList()
        };
    }
}