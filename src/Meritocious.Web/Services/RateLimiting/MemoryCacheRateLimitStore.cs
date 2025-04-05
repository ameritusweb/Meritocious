using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Memory;

namespace Meritocious.Web.Services.RateLimiting;

public class MemoryCacheRateLimitStore : IRateLimitCounterStore
{
    private readonly IMemoryCache cache;
    private readonly IRateLimitConfiguration config;

    public MemoryCacheRateLimitStore(
        IMemoryCache cache,
        IRateLimitConfiguration config)
    {
        this.cache = cache;
        this.config = config;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken token = default)
    {
        return await Task.FromResult(cache.TryGetValue(id, out _));
    }

    public async Task<RateLimitCounter?> GetAsync(string id, CancellationToken token = default)
    {
        if (cache.TryGetValue(id, out RateLimitCounter? entry))
        {
            return await Task.FromResult(entry);
        }

        return null;
    }

    public async Task RemoveAsync(string id, CancellationToken token = default)
    {
        cache.Remove(id);
        await Task.CompletedTask;
    }

    public async Task SetAsync(string id, RateLimitCounter? counter, TimeSpan? expirationTime, 
        CancellationToken token = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        };

        cache.Set(id, counter, options);
        await Task.CompletedTask;
    }
}