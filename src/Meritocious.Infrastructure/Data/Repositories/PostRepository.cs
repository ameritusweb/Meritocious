﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Recommendations.Models;

    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<Post>> GetTopPostsAsync(int count = 10)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(p => p.Tags)
                .Where(p => p.AuthorId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByTagAsync(string tagName)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Name == tagName) && !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetForkedPostsAsync(Guid parentPostId)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Where(p => p.ParentPostId == parentPostId && !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByTopicAsync(string topic, DateTime startTime)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Name == topic) &&
                           p.CreatedAt >= startTime &&
                           !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsAfterDateAsync(DateTime date)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.CreatedAt >= date && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserInteractionHistory>> GetUserInteractionHistoryAsync(Guid userId)
        {
            var interactions = await _context.UserContentInteractions
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.InteractedAt)
                .Take(100)  // Limit to recent history
                .ToListAsync();

            return interactions.Select(i => new UserInteractionHistory
            {
                ContentId = i.ContentId,
                ContentType = i.ContentType,
                InteractionType = i.InteractionType,
                Timestamp = i.InteractedAt,
                EngagementLevel = i.EngagementScore
            }).ToList();
        }
    }
}
