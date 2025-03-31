using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;

    public class CommentRepository : GenericRepository<Comment>
    {
        public CommentRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<Comment>> GetCommentsByPostAsync(Guid postId)
        {
            return await _dbSet
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(c => c.Post)
                .Where(c => c.AuthorId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetRepliesAsync(Guid parentCommentId)
        {
            return await _dbSet
                .Include(c => c.Author)
                .Where(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore)
                .ToListAsync();
        }
    }
}