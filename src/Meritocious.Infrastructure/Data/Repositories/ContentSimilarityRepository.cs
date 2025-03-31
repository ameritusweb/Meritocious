using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ContentSimilarityRepository : GenericRepository<ContentSimilarity>
    {
        public ContentSimilarityRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ContentSimilarity>> GetSimilarContentAsync(
            Guid contentId,
            decimal minSimilarity = 0.7m)
        {
            return await _dbSet
                .Where(s => (s.ContentId1 == contentId || s.ContentId2 == contentId) &&
                           s.SimilarityScore >= minSimilarity)
                .OrderByDescending(s => s.SimilarityScore)
                .ToListAsync();
        }

        public async Task<decimal> GetContentSimilarityAsync(Guid contentId1, Guid contentId2)
        {
            var similarity = await _dbSet
                .FirstOrDefaultAsync(s =>
                    (s.ContentId1 == contentId1 && s.ContentId2 == contentId2) ||
                    (s.ContentId1 == contentId2 && s.ContentId2 == contentId1));

            return similarity?.SimilarityScore ?? 0;
        }
    }

}
