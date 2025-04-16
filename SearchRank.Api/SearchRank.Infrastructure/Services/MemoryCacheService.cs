using Microsoft.Extensions.Caching.Memory;
using SearchRank.Domain.Interfaces;

namespace SearchRank.Infrastructure.Services;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    public bool TryGetValue<T>(string key, out T? value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }
    
    public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
        };

        _memoryCache.Set(key, value, options);
    }
    
    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}