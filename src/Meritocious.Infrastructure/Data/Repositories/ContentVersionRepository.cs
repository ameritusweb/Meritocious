using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Versioning.Models;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using Meritocious.Common.Enums;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ContentVersionRepository : GenericRepository<ContentVersion>
    {
        public ContentVersionRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ContentVersion>> GetVersionHistoryAsync(
            Guid contentId,
            ContentType contentType,
            bool includeDiffs = false)
        {
            var query = _dbSet
                .Include(v => v.Editor)
                .Where(v => v.ContentId == contentId && v.ContentType == contentType);

            if (includeDiffs)
            {
                query = query.Include("ContentDiffs");
            }

            return await query
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();
        }

        public async Task<ContentVersion> GetVersionAsync(
            Guid contentId,
            ContentType contentType,
            int versionNumber)
        {
            return await _dbSet
                .Include(v => v.Editor)
                .FirstOrDefaultAsync(v =>
                    v.ContentId == contentId &&
                    v.ContentType == contentType &&
                    v.VersionNumber == versionNumber);
        }

        public async Task<ContentVersion> GetLatestVersionAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Include(v => v.Editor)
                .Where(v => v.ContentId == contentId && v.ContentType == contentType)
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ContentVersion>> GetModerationEditsAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Include(v => v.Editor)
                .Where(v =>
                    v.ContentId == contentId &&
                    v.ContentType == contentType &&
                    v.IsModerationEdit)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();
        }

        public async Task<List<ContentVersion>> GetUserEditsAsync(
            Guid userId,
            DateTime? since = null)
        {
            var query = _dbSet
                .Include(v => v.Editor)
                .Where(v => v.EditorId == userId);

            if (since.HasValue)
            {
                query = query.Where(v => v.CreatedAt >= since.Value);
            }

            return await query
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContentDiff> GetVersionDiffAsync(Guid versionId)
        {
            return await _context.Set<ContentDiff>()
                .Include(d => d.ContentVersion)
                .FirstOrDefaultAsync(d => d.ContentVersionId == versionId);
        }

        public async Task<int> GetNextVersionNumberAsync(
            Guid contentId,
            ContentType contentType)
        {
            var maxVersion = await _dbSet
                .Where(v => v.ContentId == contentId && v.ContentType == contentType)
                .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

            return maxVersion + 1;
        }

        public async Task SaveDiffAsync(ContentDiff diff)
        {
            await _context.Set<ContentDiff>().AddAsync(diff);
            await _context.SaveChangesAsync();
        }
    }
}