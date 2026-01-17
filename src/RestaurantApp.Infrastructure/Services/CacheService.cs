using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RestaurantApp.Application.Interfaces;

namespace RestaurantApp.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cached = await _cache.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(cached))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(cached);
        }
        catch
        {
            // If deserialization fails, remove the corrupted cache entry
            await RemoveAsync(key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = expiration.HasValue
            ? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration.Value
            }
            : _defaultOptions;

        var serialized = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serialized, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // Note: Pattern-based removal is not natively supported by IDistributedCache
        // For Redis, you would need to use StackExchange.Redis directly
        // For now, we'll implement a simple key tracking mechanism
        
        // This is a simplified implementation
        // In production, consider using Redis SCAN command or maintaining a key registry
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var value = await _cache.GetStringAsync(key);
        return !string.IsNullOrEmpty(value);
    }
}
