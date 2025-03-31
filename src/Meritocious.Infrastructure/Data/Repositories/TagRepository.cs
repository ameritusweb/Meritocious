using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Models;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class TagRepository : GenericRepository<Tag>
    {
        public TagRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<Tag> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(t => t.ParentTag)
                .Include(t => t.Synonyms)
                .Include(t => t.RelatedTags)
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int count = 10)
        {
            return await _dbSet
                .Where(t => t.Status == TagStatus.Active)
                .OrderByDescending(t => t.UseCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetTagsByCategoryAsync(TagCategory category)
        {
            return await _dbSet
                .Include(t => t.ParentTag)
            .Where(t =>
                    t.Category == category &&
                    t.Status == TagStatus.Active)
                .OrderByDescending(t => t.UseCount)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetChildTagsAsync(Guid parentTagId)
        {
            return await _dbSet
                .Where(t =>
                    t.ParentTagId == parentTagId &&
                    t.Status == TagStatus.Active)
                .OrderByDescending(t => t.UseCount)
                .ToListAsync();
        }

        public async Task<List<Tag>> SearchTagsAsync(
            string searchTerm,
            bool includeSynonyms = true)
        {
            var query = _dbSet.AsQueryable();

            if (includeSynonyms)
            {
                query = query.Include(t => t.Synonyms);
            }

            var searchableText = searchTerm.ToLower();
            return await query
                .Where(t =>
                    t.Status == TagStatus.Active &&
                    (t.Name.ToLower().Contains(searchableText) ||
                     t.Description.ToLower().Contains(searchableText) ||
                     (includeSynonyms && t.Synonyms.Any(s =>
                         s.Name.ToLower().Contains(searchableText)))))
                .OrderByDescending(t => t.UseCount)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetRelatedTagsAsync(
            Guid tagId,
            TagRelationType? relationType = null)
        {
            var query = _context.Set<TagRelationship>()
                .Include(r => r.RelatedTag)
                .Where(r =>
                    r.SourceTagId == tagId &&
                    r.IsApproved);

            if (relationType.HasValue)
            {
                query = query.Where(r => r.RelationType == relationType.Value);
            }

            var relationships = await query.ToListAsync();
            return relationships.Select(r => r.RelatedTag).ToList();
        }

        public async Task<List<Tag>> GetTagsWithMinimumMeritAsync(decimal minMeritScore)
        {
            return await _dbSet
                .Where(t =>
                    t.Status == TagStatus.Active &&
                    t.MeritThreshold >= minMeritScore)
                .OrderByDescending(t => t.MeritThreshold)
                .ToListAsync();
        }
    }

    public class TagSynonymRepository : GenericRepository<TagSynonym>
    {
        public TagSynonymRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<TagSynonym>> GetTagSynonymsAsync(Guid tagId)
        {
            return await _dbSet
                .Include(s => s.Creator)
                .Include(s => s.ApprovedBy)
                .Where(s => s.TagId == tagId)
                .OrderByDescending(s => s.UseCount)
                .ToListAsync();
        }

        public async Task<List<TagSynonym>> GetPendingApprovalAsync()
        {
            return await _dbSet
                .Include(s => s.Tag)
                .Include(s => s.Creator)
                .Where(s => !s.IsApproved)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<TagSynonym> FindSynonymAsync(string name)
        {
            return await _dbSet
                .Include(s => s.Tag)
                .FirstOrDefaultAsync(s =>
                    s.Name.ToLower() == name.ToLower() &&
                    s.IsApproved);
        }
    }

    public class TagRelationshipRepository : GenericRepository<TagRelationship>
    {
        public TagRelationshipRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<TagRelationship>> GetTagRelationshipsAsync(
            Guid tagId,
            bool includeIncoming = true)
        {
            var query = _dbSet
                .Include(r => r.SourceTag)
                .Include(r => r.RelatedTag)
                .Include(r => r.Creator)
                .Where(r => r.IsApproved);

            if (includeIncoming)
            {
                query = query.Where(r =>
                    r.SourceTagId == tagId ||
                    (r.RelatedTagId == tagId && r.IsBidirectional));
            }
            else
            {
                query = query.Where(r => r.SourceTagId == tagId);
            }

            return await query
                .OrderByDescending(r => r.Strength)
                .ToListAsync();
        }

        public async Task<List<TagRelationship>> GetPendingApprovalAsync()
        {
            return await _dbSet
                .Include(r => r.SourceTag)
                .Include(r => r.RelatedTag)
                .Include(r => r.Creator)
                .Where(r => !r.IsApproved)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TagRelationship>> GetRelationshipsByTypeAsync(
            TagRelationType relationType)
        {
            return await _dbSet
                .Include(r => r.SourceTag)
                .Include(r => r.RelatedTag)
                .Where(r =>
                    r.RelationType == relationType &&
                    r.IsApproved)
                .OrderByDescending(r => r.Strength)
                .ToListAsync();
        }
    }

    public class TagWikiRepository : GenericRepository<TagWiki>
    {
        public TagWikiRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<TagWiki>> GetTagWikiHistoryAsync(Guid tagId)
        {
            return await _dbSet
                .Include(w => w.Editor)
                .Include(w => w.ApprovedBy)
                .Where(w => w.TagId == tagId)
                .OrderByDescending(w => w.VersionNumber)
                .ToListAsync();
        }

        public async Task<TagWiki> GetLatestWikiVersionAsync(Guid tagId)
        {
            return await _dbSet
                .Include(w => w.Editor)
                .Include(w => w.ApprovedBy)
                .Where(w => w.TagId == tagId && w.IsApproved)
                .OrderByDescending(w => w.VersionNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<List<TagWiki>> GetPendingApprovalAsync()
        {
            return await _dbSet
                .Include(w => w.Tag)
                .Include(w => w.Editor)
                .Where(w => !w.IsApproved)
                .OrderBy(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetNextVersionNumberAsync(Guid tagId)
        {
            var maxVersion = await _dbSet
                .Where(w => w.TagId == tagId)
                .MaxAsync(w => (int?)w.VersionNumber) ?? 0;

            return maxVersion + 1;
        }

        public async Task<List<TagWiki>> GetRecentChangesAsync(
            int count = 10,
            bool approvedOnly = true)
        {
            var query = _dbSet
                .Include(w => w.Tag)
                .Include(w => w.Editor)
                .Include(w => w.ApprovedBy);

            if (approvedOnly)
            {
                query = query.Where(w => w.IsApproved);
            }

            return await query
                .OrderByDescending(w => w.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}