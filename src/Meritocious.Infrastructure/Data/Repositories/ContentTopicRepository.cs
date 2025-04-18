﻿using Meritocious.Common.Enums;
using Meritocious.Core.Features.Recommendations.Models;
using Meritocious.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ContentTopicRepository : GenericRepository<ContentTopic>, IContentTopicRepository
    {
        public ContentTopicRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ContentTopic>> GetContentTopicsAsync(
            string contentId,
            ContentType contentType)
        {
            return await dbSet
                .Where(t => t.ContentId == contentId && t.ContentType == contentType)
                .OrderByDescending(t => t.Relevance)
                .ToListAsync();
        }

        public Task<List<ContentTopic>> GetTopicContentAsync(string topic, int limit, decimal minRelevance)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }

        public async Task<List<ContentTopic>> GetTopicsForContentListAsync(
            List<string> contentIds,
            ContentType contentType)
        {
            return await dbSet
                .Where(t => contentIds.Contains(t.ContentId) && t.ContentType == contentType)
                .OrderByDescending(t => t.Relevance)
                .ToListAsync();
        }

        public async Task<List<string>> GetTopTrendingTopicsAsync(int count = 10)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            return await dbSet
                .Where(t => t.ExtractedAt >= thirtyDaysAgo)
                .GroupBy(t => t.Topic)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();
        }
    }
}
