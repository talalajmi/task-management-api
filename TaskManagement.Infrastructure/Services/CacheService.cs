using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class CacheService(IDistributedCache cache, IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDistributedCache _cache = cache;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<T?> GetAsync<T>(string key)
    {
        var cached = await _cache.GetStringAsync(key);

        if (cached is null)
            return default; // Cache miss — return null

        // Deserialize from JSON back to the requested type
        return JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            // Default cache expiration: 10 minutes
            // After this, the cache entry expires automatically
            // and the next request will hit the database
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10),
        };

        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // StackExchange.Redis allows us to search by pattern
        // This is how we invalidate groups of related cache entries
        var server = _redis.GetServer(_redis.GetEndPoints().First());

        var keys = server.Keys(pattern: pattern).ToArray();

        foreach (var key in keys)
        {
            await _cache.RemoveAsync(key.ToString()!);
        }
    }
}
