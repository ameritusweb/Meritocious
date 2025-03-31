using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meritocious.AI.SemanticClustering.Services;
using Meritocious.AI.Recommendations;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Infrastructure.Data.Extensions
{
    public static class PostRepositoryExtensions
    {
        public static async Task<List<Post>> GetTopPostsByMeritAsync(
            this PostRepository repository,
            int count = 10)
        {
            return await repository.DbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<List<Post>> GetLatestPostsAsync(
            this PostRepository repository,
            int count = 10)
        {
            return await repository.DbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<List<Post>> GetMostActivePostsAsync(
            this PostRepository repository,
            int count = 10)
        {
            return await repository.DbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.Comments.Count)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<List<Post>> GetPostsAfterDateAsync(
            this PostRepository repository,
            DateTime startDate)
        {
            return await repository.DbSet
                .Include(p => p.Author)
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate)
                .ToListAsync();
        }

        public static async Task<List<Post>> GetPostsByTopicAsync(
            this PostRepository repository,
            string topic,
            DateTime? startDate = null)
        {
            var query = repository.DbSet
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Where(p => !p.IsDeleted);

            if (startDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= startDate.Value);
            }

            // Simple keyword search - this would be replaced with semantic search
            query = query.Where(p => p.Title.Contains(topic) ||
                                    p.Content.Contains(topic) ||
                                    p.Tags.Any(t => t.Name == topic));

            return await query.ToListAsync();
        }

        public static async Task<List<UserInteractionHistory>> GetUserInteractionHistoryAsync(
            this PostRepository repository,
            Guid userId)
        {
            // Get posts the user has interacted with
            var posts = await repository.DbSet
                .Include(p => p.Comments.Where(c => c.AuthorId == userId))
                .Where(p => p.AuthorId == userId || p.Comments.Any(c => c.AuthorId == userId))
                .ToListAsync();

            var history = new List<UserInteractionHistory>();

            // Add authored posts
            foreach (var post in posts.Where(p => p.AuthorId == userId))
            {
                history.Add(new UserInteractionHistory
                {
                    ContentId = post.Id,
                    InteractionType = "author",
                    Timestamp = post.CreatedAt,
                    EngagementLevel = 1.0m
                });
            }

            // Add commented posts
            foreach (var post in posts.Where(p => p.Comments.Any()))
            {
                foreach (var comment in post.Comments)
                {
                    history.Add(new UserInteractionHistory
                    {
                        ContentId = post.Id,
                        InteractionType = "comment",
                        Timestamp = comment.CreatedAt,
                        EngagementLevel = 0.7m
                    });
                }
            }

            return history;
        }
    }
}