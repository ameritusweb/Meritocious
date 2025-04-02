using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;

namespace Meritocious.Infrastructure.Data.Repositories;

public class RemixSourceRepository : GenericRepository<RemixSource>, IRemixSourceRepository
{
    private readonly MeritociousDbContext _context;

    public RemixSourceRepository(MeritociousDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RemixSource> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Set<RemixSource>()
            .Include(rs => rs.SourcePost)
            .Include(rs => rs.Remix)
            .FirstOrDefaultAsync(rs => rs.Id == id);
    }

    public async Task<IEnumerable<RemixSource>> GetByRemixIdAsync(Guid remixId)
    {
        return await _context.Set<RemixSource>()
            .Include(rs => rs.SourcePost)
            .Where(rs => rs.RemixId == remixId)
            .OrderBy(rs => rs.Order)
            .ToListAsync();
    }

    public async Task<IEnumerable<RemixSource>> GetByPostIdAsync(Guid postId)
    {
        return await _context.Set<RemixSource>()
            .Include(rs => rs.Remix)
            .Where(rs => rs.SourcePostId == postId)
            .OrderByDescending(rs => rs.Remix.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateQuotesAsync(Guid sourceId, IEnumerable<string> quotes)
    {
        var source = await _context.Set<RemixSource>()
            .FirstOrDefaultAsync(rs => rs.Id == sourceId);

        if (source != null)
        {
            source.QuotedExcerpts = quotes.ToList();
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateRelevanceScoresAsync(Guid sourceId, Dictionary<string, decimal> scores)
    {
        var source = await _context.Set<RemixSource>()
            .FirstOrDefaultAsync(rs => rs.Id == sourceId);

        if (source != null)
        {
            source.RelevanceScores = scores;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<string, int>> GetRelationshipDistributionAsync(Guid remixId)
    {
        var distribution = await _context.Set<RemixSource>()
            .Where(rs => rs.RemixId == remixId)
            .GroupBy(rs => rs.Relationship)
            .Select(g => new { Relationship = g.Key, Count = g.Count() })
            .ToListAsync();

        return distribution.ToDictionary(x => x.Relationship, x => x.Count);
    }
}