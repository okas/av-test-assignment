using Microsoft.Extensions.Caching.Memory;

namespace Backend.WebApi.App.Cache;

public interface ICacheService<T>
{
    Task<T> Get(string key);

    void Set(string key, T entry, MemoryCacheEntryOptions? options = default);

    void Remove(string key);
}
