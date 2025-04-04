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
    public interface IUserInteractionRepository
    {
        Task<List<UserContentInteraction>> GetUserInteractionsAsync(Guid userId, DateTime? since = null);
        Task<List<UserContentInteraction>> GetContentInteractionsAsync(Guid contentId, ContentType contentType, DateTime? since = null);
        Task<Dictionary<string, decimal>> GetUserInteractionPatternsAsync(Guid userId);
    }

    public class UserInteractionRepository : GenericRepository<UserContentInteraction>
    {
        public UserInteractionRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<UserContentInteraction>> GetUserInteractionsAsync(
            Guid userId,
            DateTime? since = null)
        {
            var query = dbSet
                .Include(i => i.User)
                .Where(i => i.UserId == userId);

            if (since.HasValue)
            {
                query = query.Where(i => i.InteractedAt >= since.Value);
            }

            return await query
                .OrderByDescending(i => i.InteractedAt)
                .ToListAsync();
        }

        public async Task<List<UserContentInteraction>> GetContentInteractionsAsync(
            Guid contentId,
            ContentType contentType,
            DateTime? since = null)
        {
            var query = dbSet
                .Include(i => i.User)
                .Where(i => i.ContentId == contentId && i.ContentType == contentType);

            if (since.HasValue)
            {
                query = query.Where(i => i.InteractedAt >= since.Value);
            }

            return await query
                .OrderByDescending(i => i.InteractedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetUserInteractionPatternsAsync(Guid userId)
        {
            var interactions = await dbSet
                .Where(i => i.UserId == userId)
                .GroupBy(i => i.InteractionType)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var total = interactions.Sum(i => i.Count);
            return interactions.ToDictionary(
                i => i.Type,
                i => (decimal)i.Count / total);
        }
    }
}
