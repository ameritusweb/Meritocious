using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Common.DTOs.Merit;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetUserMeritScoreQueryHandler : IRequestHandler<GetUserMeritScoreQuery, MeritScoreDto>
{
    private readonly MeritociousDbContext context;

    public GetUserMeritScoreQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<MeritScoreDto> Handle(GetUserMeritScoreQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.MeritScoreHistories)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User {request.UserId} not found");
        }

        return new MeritScoreDto
        {
            UserId = Guid.Parse(user.Id),
            CurrentScore = user.MeritScore,
            LastCalculated = user.LastCalculated.GetValueOrDefault(),
            ScoreHistory = user.MeritScoreHistories.Select(h => new MeritScoreHistoryDto
            {
                Score = h.Score,
                Timestamp = h.Timestamp,
                Reason = h.Reason
            }).ToList()
        };
    }
}