using Microsoft.EntityFrameworkCore;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using Meritocious.Common.Enums;
using Meritocious.Core.Features.Reputation.Models;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class MeritScoreHistoryRepository : GenericRepository<MeritScoreHistory>
    {
        public MeritScoreHistoryRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<MeritScoreHistory>> GetContentScoreHistoryAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(h => h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.EvaluatedAt)
                .ToListAsync();
        }

        public async Task<MeritScoreHistory> GetLatestScoreAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(h => h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.EvaluatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<MeritScoreHistory>> GetRecalculationsAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(h =>
                    h.ContentId == contentId &&
                    h.ContentType == contentType &&
                    h.IsRecalculation)
                .OrderByDescending(h => h.EvaluatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetAverageComponentScoresAsync(
            DateTime start,
            DateTime end)
        {
            var scores = await _dbSet
                .Where(h => h.EvaluatedAt >= start && h.EvaluatedAt <= end)
                .SelectMany(h => h.Components)
                .GroupBy(c => c.Key)
                .Select(g => new
                {
                    Component = g.Key,
                    Average = g.Average(c => c.Value)
                })
                .ToListAsync();

            return scores.ToDictionary(s => s.Component, s => s.Average);
        }
    }
}