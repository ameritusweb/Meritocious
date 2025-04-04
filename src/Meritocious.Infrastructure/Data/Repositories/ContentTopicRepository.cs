using Meritocious.Common.Enums;
using Meritocious.Core.Features.Recommendations.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public interface IContentTopicRepository
    {
        Task<List<ContentTopic>> GetContentTopicsAsync(Guid contentId, ContentType contentType);
        Task<List<ContentTopic>> GetTopicsForContentListAsync(List<Guid> contentIds, ContentType contentType);
        Task<List<string>> GetTopTrendingTopicsAsync(int count = 10);
    }

    public class ContentTopicRepository : GenericRepository<ContentTopic>
    {
        public ContentTopicRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ContentTopic>> GetContentTopicsAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(t => t.ContentId == contentId && t.ContentType == contentType)
                .OrderByDescending(t => t.Relevance)
                .ToListAsync();
        }

        public async Task<List<ContentTopic>> GetTopicsForContentListAsync(
            List<Guid> contentIds,
            ContentType contentType)
        {
            return await _dbSet
                .Where(t => contentIds.Contains(t.ContentId) && t.ContentType == contentType)
                .OrderByDescending(t => t.Relevance)
                .ToListAsync();
        }

        public async Task<List<string>> GetTopTrendingTopicsAsync(int count = 10)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            return await _dbSet
                .Where(t => t.ExtractedAt >= thirtyDaysAgo)
                .GroupBy(t => t.Topic)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();
        }
    }
}
