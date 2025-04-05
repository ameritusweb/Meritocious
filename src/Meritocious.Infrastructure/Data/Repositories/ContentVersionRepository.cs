using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Common.Enums;
using Meritocious.Core.Features.Versioning;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public interface IContentVersionRepository
    {
        Task<List<ContentVersion>> GetVersionHistoryAsync(string contentId, ContentType contentType, bool includeDiffs = false);
        Task<ContentVersion> GetVersionAsync(string contentId, ContentType contentType, int versionNumber);
        Task<ContentVersion> GetLatestVersionAsync(string contentId, ContentType contentType);
        Task<List<ContentVersion>> GetModerationEditsAsync(string contentId, ContentType contentType);
        Task<List<ContentVersion>> GetUserEditsAsync(string userId, DateTime? since = null);
        Task<ContentDiff> GetVersionDiffAsync(string versionId);
        Task<int> GetNextVersionNumberAsync(string contentId, ContentType contentType);
        Task SaveDiffAsync(ContentDiff diff);
    }

    public class ContentVersionRepository : GenericRepository<ContentVersion>
    {
        public ContentVersionRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ContentVersion>> GetVersionHistoryAsync(
            string contentId,
            ContentType contentType,
            bool includeDiffs = false)
        {
            var query = dbSet
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
            string contentId,
            ContentType contentType,
            int versionNumber)
        {
            return await dbSet
                .Include(v => v.Editor)
                .FirstOrDefaultAsync(v =>
                    v.ContentId == contentId &&
                    v.ContentType == contentType &&
                    v.VersionNumber == versionNumber);
        }

        public async Task<ContentVersion> GetLatestVersionAsync(
            string contentId,
            ContentType contentType)
        {
            return await dbSet
                .Include(v => v.Editor)
                .Where(v => v.ContentId == contentId && v.ContentType == contentType)
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ContentVersion>> GetModerationEditsAsync(
            string contentId,
            ContentType contentType)
        {
            return await dbSet
                .Include(v => v.Editor)
                .Where(v =>
                    v.ContentId == contentId &&
                    v.ContentType == contentType &&
                    v.IsModerationEdit)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();
        }

        public async Task<List<ContentVersion>> GetUserEditsAsync(
            string userId,
            DateTime? since = null)
        {
            var query = dbSet
                .Include(v => v.Editor)
                .Where(v => v.EditorId == userId.ToString());

            if (since.HasValue)
            {
                query = query.Where(v => v.CreatedAt >= since.Value);
            }

            return await query
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContentDiff> GetVersionDiffAsync(string versionId)
        {
            return await context.Set<ContentDiff>()
                .Include(d => d.ContentVersion)
                .FirstOrDefaultAsync(d => d.ContentVersionId == versionId);
        }

        public async Task<int> GetNextVersionNumberAsync(
            string contentId,
            ContentType contentType)
        {
            var maxVersion = await dbSet
                .Where(v => v.ContentId == contentId && v.ContentType == contentType)
                .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

            return maxVersion + 1;
        }

        public async Task SaveDiffAsync(ContentDiff diff)
        {
            await context.Set<ContentDiff>().AddAsync(diff);
            await context.SaveChangesAsync();
        }
    }
}