using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;

namespace Meritocious.Infrastructure.Data.Repositories;

public class RemixNoteRepository : GenericRepository<RemixNote>, IRemixNoteRepository
{
    private readonly MeritociousDbContext _context;

    public RemixNoteRepository(MeritociousDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RemixNote>> GetByRemixIdAsync(Guid remixId)
    {
        return await _context.Set<RemixNote>()
            .Where(rn => rn.RemixId == remixId)
            .OrderByDescending(rn => rn.Confidence)
            .ToListAsync();
    }

    public async Task<IEnumerable<RemixNote>> GetBySourceIdAsync(Guid sourceId)
    {
        return await _context.Set<RemixNote>()
            .Where(rn => rn.RelatedSourceIds.Contains(sourceId))
            .OrderByDescending(rn => rn.Confidence)
            .ToListAsync();
    }

    public async Task<IEnumerable<RemixNote>> GetHighConfidenceNotesAsync(Guid remixId, decimal minConfidence = 0.8m)
    {
        return await _context.Set<RemixNote>()
            .Where(rn => rn.RemixId == remixId && rn.Confidence >= minConfidence)
            .OrderByDescending(rn => rn.Confidence)
            .ToListAsync();
    }

    public async Task<IEnumerable<RemixNote>> GetUnusedSuggestionsAsync(Guid remixId)
    {
        return await _context.Set<RemixNote>()
            .Where(rn => rn.RemixId == remixId && !rn.IsApplied)
            .OrderByDescending(rn => rn.Confidence)
            .ToListAsync();
    }

    public async Task MarkNoteAppliedAsync(Guid noteId, bool isApplied)
    {
        var note = await _context.Set<RemixNote>()
            .FirstOrDefaultAsync(rn => rn.Id == noteId);

        if (note != null)
        {
            note.IsApplied = isApplied;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<string, int>> GetNoteTypeDistributionAsync(Guid remixId)
    {
        var distribution = await _context.Set<RemixNote>()
            .Where(rn => rn.RemixId == remixId)
            .GroupBy(rn => rn.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        return distribution.ToDictionary(x => x.Type, x => x.Count);
    }
}