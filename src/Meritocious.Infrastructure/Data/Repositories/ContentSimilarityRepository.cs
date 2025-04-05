using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meritocious.Core.Interfaces;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ContentSimilarityRepository : GenericRepository<ContentSimilarity>, IContentSimilarityRepository
    {
        public ContentSimilarityRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ContentSimilarity>> GetSimilarContentAsync(
            string contentId,
            decimal minSimilarity = 0.7m)
        {
            return await dbSet
                .Where(s => (s.ContentId1 == contentId || s.ContentId2 == contentId) &&
                           s.SimilarityScore >= minSimilarity)
                .OrderByDescending(s => s.SimilarityScore)
                .ToListAsync();
        }

        public async Task<decimal> GetContentSimilarityAsync(string contentId1, string contentId2)
        {
            var similarity = await dbSet
                .FirstOrDefaultAsync(s =>
                    (s.ContentId1 == contentId1 && s.ContentId2 == contentId2) ||
                    (s.ContentId1 == contentId2 && s.ContentId2 == contentId1));

            return similarity?.SimilarityScore ?? 0;
        }

        public async Task<List<(string id1, string id2)>> GetContentPairsForUpdateAsync(int batchSize = 100)
        {
            var result = await dbSet
                .Where(s => s.NeedsUpdate)
                .OrderByDescending(s => s.UpdatePriority)
                .ThenBy(s => s.LastUpdated)
                .Select(s => new { s.ContentId1, s.ContentId2 })
                .Take(batchSize)
                .AsNoTracking()
                .ToListAsync();

            return result.Select(x => (x.ContentId1, x.ContentId2))
                .ToList();
        }

        public async Task MarkForUpdateAsync(string contentId, int priority = 0)
        {
            // Mark all pairs involving this content
            var similarities = await dbSet
                .Where(s => s.ContentId1 == contentId || s.ContentId2 == contentId)
                .ToListAsync();

            foreach (var similarity in similarities)
            {
                similarity.MarkForUpdate(priority);
            }

            await context.SaveChangesAsync();
        }

        public async Task MarkOldSimilaritiesForUpdateAsync(TimeSpan age, int priority = 0)
        {
            var cutoff = DateTime.UtcNow - age;
            var oldSimilarities = await dbSet
                .Where(s => s.LastUpdated < cutoff && !s.NeedsUpdate)
                .ToListAsync();

            foreach (var similarity in oldSimilarities)
            {
                similarity.MarkForUpdate(priority);
            }

            await context.SaveChangesAsync();
        }

        public async Task CreateMissingSimilaritiesAsync(List<string> contentIds)
        {
            // Find all existing pairs
            var existingPairs = await dbSet
                .Where(s => contentIds.Contains(s.ContentId1) && contentIds.Contains(s.ContentId2))
                .Select(s => new { s.ContentId1, s.ContentId2 })
                .ToListAsync();

            var existingSet = new HashSet<string>(existingPairs.Select(p => 
                GetPairKey(p.ContentId1, p.ContentId2)));

            // Generate all possible pairs
            var newPairs = new List<ContentSimilarity>();
            for (int i = 0; i < contentIds.Count; i++)
            {
                for (int j = i + 1; j < contentIds.Count; j++)
                {
                    var id1 = contentIds[i];
                    var id2 = contentIds[j];
                    var key = GetPairKey(id1, id2);

                    if (!existingSet.Contains(key))
                    {
                        newPairs.Add(ContentSimilarity.Create(id1, id2, 0));
                    }
                }
            }

            if (newPairs.Any())
            {
                await dbSet.AddRangeAsync(newPairs);
                await context.SaveChangesAsync();
            }
        }

        private static string GetPairKey(string id1, string id2)
        {
            // Ensure consistent ordering
            var ordered = new[] { id1, id2 }.OrderBy(id => id).ToList();
            return $"{ordered[0]}:{ordered[1]}";
        }
    }
}
