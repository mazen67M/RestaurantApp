namespace RestaurantApp.Application.Interfaces;

/// <summary>
/// Distributed cache service for improving performance
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get cached value by key
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Set cache value with optional expiration
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Remove cached value by key
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Remove all cached values matching a pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key);
}
