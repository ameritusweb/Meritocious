
namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;

    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByPostAsync(Guid postId);
        Task<List<Comment>> GetCommentsByUserAsync(Guid userId);
        Task<List<Comment>> GetRepliesAsync(Guid parentCommentId);
        Task<List<Comment>> GetCommentsByPostOrderedByMeritAsync(Guid postId, int? page = null, int? pageSize = null);
        Task<List<Comment>> GetCommentsByPostOrderedByDateAsync(Guid postId, int? page = null, int? pageSize = null);
        Task<List<Comment>> GetCommentsByPostThreadedAsync(Guid postId, int? page = null, int? pageSize = null);
    }

    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<Comment>> GetCommentsByPostAsync(Guid postId)
        {
            return await dbSet
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByUserAsync(Guid userId)
        {
            return await dbSet
                .Include(c => c.Post)
                .Where(c => c.AuthorId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetRepliesAsync(Guid parentCommentId)
        {
            return await dbSet
                .Include(c => c.Author)
                .Where(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByPostOrderedByMeritAsync(
    Guid postId,
    int? page = null,
    int? pageSize = null)
        {
            var query = context.Comments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .OrderByDescending(c => c.MeritScore);

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value)
                            .Take(pageSize.Value)
                             .OrderByDescending(c => c.MeritScore); ;
            }

            return await query.ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByPostOrderedByDateAsync(
            Guid postId,
            int? page = null,
            int? pageSize = null)
        {
            var query = context.Comments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt);

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value)
                            .Take(pageSize.Value)
                             .OrderByDescending(c => c.CreatedAt);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByPostThreadedAsync(
            Guid postId,
            int? page = null,
            int? pageSize = null)
        {
            var query = context.Comments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId &&
                           !c.IsDeleted &&
                           !c.ParentCommentId.HasValue)  // Root comments only
                .OrderByDescending(c => c.MeritScore);

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value)
                            .Take(pageSize.Value)
                            .OrderByDescending(c => c.MeritScore);
            }

            return await query.ToListAsync();
        }
    }
}