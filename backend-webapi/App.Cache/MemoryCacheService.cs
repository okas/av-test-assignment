using Microsoft.Extensions.Caching.Memory;

namespace Backend.WebApi.App.Cache;

public class MemoryCacheService<T> : ICacheService<T>
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache memoryCache) => _cache = memoryCache;

    public virtual Task<T> Get(string key)
    {
        _cache.TryGetValue(key, out T entry);

        return Task.FromResult(entry);
    }

    public void Set(string key, T entry, MemoryCacheEntryOptions? options = null) =>
        _cache.Set(key, entry, options ?? new MemoryCacheEntryOptions());

    public void Remove(string key) => _cache.Remove(key);
}
