﻿using Microsoft.EntityFrameworkCore;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using Meritocious.Common.Enums;
using Meritocious.Core.Features.Reputation.Models;
using Meritocious.Core.Interfaces;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public interface IMeritScoreHistoryRepository : IRepository<MeritScoreHistory>
    {
        Task<List<MeritScoreHistory>> GetContentScoreHistoryAsync(string contentId, ContentType contentType);
        Task<MeritScoreHistory> GetLatestScoreAsync(string contentId, ContentType contentType);
        Task<List<MeritScoreHistory>> GetRecalculationsAsync(string contentId, ContentType contentType);
        Task<Dictionary<string, decimal>> GetAverageComponentScoresAsync(DateTime start, DateTime end);
    }

    public class MeritScoreHistoryRepository : GenericRepository<MeritScoreHistory>, IMeritScoreHistoryRepository
    {
        public MeritScoreHistoryRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<MeritScoreHistory>> GetContentScoreHistoryAsync(
            string contentId,
            ContentType contentType)
        {
            return await dbSet
                .Where(h => h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.EvaluatedAt)
                .ToListAsync();
        }

        public async Task<MeritScoreHistory> GetLatestScoreAsync(
            string contentId,
            ContentType contentType)
        {
            return await dbSet
                .Where(h => h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.EvaluatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<MeritScoreHistory>> GetRecalculationsAsync(
            string contentId,
            ContentType contentType)
        {
            return await dbSet
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
            var scores = await dbSet
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