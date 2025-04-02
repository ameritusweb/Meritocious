using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;

namespace Meritocious.Infrastructure.Data.Repositories;

public class RemixRepository : GenericRepository<Remix>, IRemixRepository
{
    private readonly MeritociousDbContext _context;

    public RemixRepository(MeritociousDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Remix> GetByIdWithSourcesAsync(Guid id)
    {
        return await _context.Set<Remix>()
            .Include(r => r.Sources)
                .ThenInclude(s => s.SourcePost)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Remix> GetByIdWithFullDetailsAsync(Guid id)
    {
        return await _context.Set<Remix>()
            .Include(r => r.Sources)
                .ThenInclude(s => s.SourcePost)
            .Include(r => r.Notes)
            .Include(r => r.Tags)
            .Include(r => r.Author)
            .Include(r => r.Versions)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Remix>> GetUserRemixesAsync(Guid userId, bool includeDrafts = false)
    {
        var query = _context.Set<Remix>()
            .Include(r => r.Sources)
            .Where(r => r.AuthorId == userId);

        if (!includeDrafts)
        {
            query = query.Where(r => !r.IsDraft);
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Remix>> GetRelatedRemixesAsync(Guid remixId, int limit = 5)
    {
        // Get tags of the current remix
        var currentRemixTags = await _context.Set<Remix>()
            .Where(r => r.Id == remixId)
            .SelectMany(r => r.Tags.Select(t => t.Id))
            .ToListAsync();

        // Get remixes with similar tags, ordered by tag overlap
        return await _context.Set<Remix>()
            .Where(r => r.Id != remixId && !r.IsDraft)
            .Include(r => r.Tags)
            .Select(r => new
            {
                Remix = r,
                TagOverlap = r.Tags.Count(t => currentRemixTags.Contains(t.Id))
            })
            .OrderByDescending(x => x.TagOverlap)
            .ThenByDescending(x => x.Remix.MeritScore)
            .Take(limit)
            .Select(x => x.Remix)
            .ToListAsync();
    }

    public async Task<IEnumerable<Remix>> GetBySourcePostAsync(Guid postId)
    {
        return await _context.Set<Remix>()
            .Include(r => r.Sources)
            .Where(r => r.Sources.Any(s => s.SourcePostId == postId))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Remix>> GetTrendingRemixesAsync(int limit = 10)
    {
        // Consider merit score, recency, and engagement
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return await _context.Set<Remix>()
            .Where(r => !r.IsDraft && r.CreatedAt >= thirtyDaysAgo)
            .OrderByDescending(r => 
                (r.MeritScore * 0.4) + 
                (EF.Functions.DateDiffDay(r.CreatedAt, DateTime.UtcNow) * -0.01))
            .Take(limit)
            .Include(r => r.Author)
            .Include(r => r.Tags)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetSourceCountsAsync(IEnumerable<Guid> remixIds)
    {
        var counts = await _context.Set<RemixSource>()
            .Where(rs => remixIds.Contains(rs.RemixId))
            .GroupBy(rs => rs.RemixId)
            .Select(g => new { RemixId = g.Key, Count = g.Count() })
            .ToListAsync();

        return counts.ToDictionary(x => x.RemixId, x => x.Count);
    }

    public async Task<bool> HasUserRemixedPostAsync(Guid userId, Guid postId)
    {
        return await _context.Set<Remix>()
            .AnyAsync(r => r.AuthorId == userId && 
                          r.Sources.Any(s => s.SourcePostId == postId));
    }

    public async Task<IEnumerable<Tag>> GetPopularRemixTagsAsync(int limit = 10)
    {
        return await _context.Set<Remix>()
            .Where(r => !r.IsDraft)
            .SelectMany(r => r.Tags)
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(limit)
            .ToListAsync();
    }

    public async Task UpdateSynthesisMapAsync(Guid remixId, string synthesisMap)
    {
        var remix = await _context.Set<Remix>()
            .FirstOrDefaultAsync(r => r.Id == remixId);

        if (remix != null)
        {
            remix.SynthesisMap = synthesisMap;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddSourceAsync(Guid remixId, RemixSource source)
    {
        var remix = await _context.Set<Remix>()
            .Include(r => r.Sources)
            .FirstOrDefaultAsync(r => r.Id == remixId);

        if (remix != null)
        {
            source.Order = remix.Sources.Count;
            remix.Sources.Add(source);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveSourceAsync(Guid remixId, Guid sourceId)
    {
        var source = await _context.Set<RemixSource>()
            .FirstOrDefaultAsync(rs => rs.RemixId == remixId && rs.Id == sourceId);

        if (source != null)
        {
            _context.Set<RemixSource>().Remove(source);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<(Guid SourceId, int NewOrder)> orderUpdates)
    {
        var sources = await _context.Set<RemixSource>()
            .Where(rs => rs.RemixId == remixId)
            .ToListAsync();

        foreach (var (sourceId, newOrder) in orderUpdates)
        {
            var source = sources.FirstOrDefault(s => s.Id == sourceId);
            if (source != null)
            {
                source.Order = newOrder;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetRemixCountAsync(Guid userId)
    {
        return await _context.Set<Remix>()
            .CountAsync(r => r.AuthorId == userId && !r.IsDraft);
    }
}