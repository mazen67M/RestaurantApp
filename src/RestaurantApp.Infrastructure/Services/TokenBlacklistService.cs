using Microsoft.Extensions.Caching.Distributed;
using RestaurantApp.Application.Interfaces;

namespace RestaurantApp.Infrastructure.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IDistributedCache _cache;

    public TokenBlacklistService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task BlacklistTokenAsync(string token, TimeSpan expiry)
    {
        await _cache.SetStringAsync(GetCacheKey(token), "revoked", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        });
    }

    public async Task<bool> IsBlacklistedAsync(string token)
    {
        return await _cache.GetStringAsync(GetCacheKey(token)) != null;
    }

    private static string GetCacheKey(string token) => $"blacklist:{token}";
}
