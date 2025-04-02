using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Memory;

namespace Meritocious.Web.Services.RateLimiting;

public class MemoryCacheRateLimitStore : IRateLimitCounterStore
{
    private readonly IMemoryCache _cache;
    private readonly IRateLimitConfiguration _config;

    public MemoryCacheRateLimitStore(
        IMemoryCache cache,
        IRateLimitConfiguration config)
    {
        _cache = cache;
        _config = config;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken token = default)
    {
        return await Task.FromResult(_cache.TryGetValue(id, out _));
    }

    public async Task<RateLimitCounter?> GetAsync(string id, CancellationToken token = default)
    {
        if (_cache.TryGetValue(id, out RateLimitCounter? entry))
        {
            return await Task.FromResult(entry);
        }

        return null;
    }

    public async Task RemoveAsync(string id, CancellationToken token = default)
    {
        _cache.Remove(id);
        await Task.CompletedTask;
    }

    public async Task SetAsync(string id, RateLimitCounter counter, TimeSpan expirationTime, 
        CancellationToken token = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        };

        _cache.Set(id, counter, options);
        await Task.CompletedTask;
    }
}