namespace TaskManagement.Application.Interfaces;

public interface ICacheService
{
    // Try to get a value from cache
    Task<T?> GetAsync<T>(string key);

    // Store a value in cache with expiration
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    // Remove a specific cache entry
    Task RemoveAsync(string key);

    // Remove all entries matching a pattern
    // e.g. "tasks:project:*" removes all project task caches
    Task RemoveByPatternAsync(string pattern);
}
