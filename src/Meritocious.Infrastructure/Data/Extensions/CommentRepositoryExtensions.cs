using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Extensions
{
    public static class CommentRepositoryExtensions
    {
        public static async Task<List<Comment>> GetCommentsByPostOrderedByMeritAsync(
            this CommentRepository repository,
            Guid postId,
            int? page = null,
            int? pageSize = null)
        {
            var query = repository._dbSet
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted && c.ParentCommentId == null)
                .OrderByDescending(c => c.MeritScore);

            // Apply pagination if needed
            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public static async Task<List<Comment>> GetCommentsByPostOrderedByDateAsync(
            this CommentRepository repository,
            Guid postId,
            int? page = null,
            int? pageSize = null)
        {
            var query = repository._dbSet
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted && c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt);

            // Apply pagination if needed
            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public static async Task<List<Comment>> GetCommentsByPostThreadedAsync(
            this CommentRepository repository,
            Guid postId,
            int? page = null,
            int? pageSize = null)
        {
            // First get all root-level comments
            var rootComments = await repository._dbSet
                .Include(c => c.Author)
                .Where(c => c.PostId == postId && !c.IsDeleted && c.ParentCommentId == null)
                .OrderByDescending(c => c.MeritScore)
                .ToListAsync();

            // Apply pagination if needed
            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                rootComments = rootComments.Skip(skip).Take(pageSize.Value).ToList();
            }

            // Now load the replies for each root comment
            foreach (var comment in rootComments)
            {
                await LoadCommentRepliesRecursivelyAsync(repository, comment);
            }

            return rootComments;
        }

        private static async Task LoadCommentRepliesRecursivelyAsync(
            CommentRepository repository,
            Comment parentComment)
        {
            var replies = await repository._dbSet
                .Include(c => c.Author)
                .Where(c => c.ParentCommentId == parentComment.Id && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore)
                .ToListAsync();

            // Add replies to parent
            foreach (var reply in replies)
            {
                // Load replies of replies recursively
                await LoadCommentRepliesRecursivelyAsync(repository, reply);
            }
        }

        public static async Task<List<Comment>> GetTopCommentsByUserAsync(
            this CommentRepository repository,
            Guid userId,
            int count = 5)
        {
            return await repository._dbSet
                .Include(c => c.Post)
                .Where(c => c.AuthorId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore)
                .Take(count)
                .ToListAsync();
        }
    }
}