using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using XCloud.Core.Cache;
using XCloud.Core.Cache.RequestCache;

namespace XCloud.AspNetMvc.RequestCache;

public class AsyncLocalRequestCache : IRequestCacheProvider
{
    private static readonly AsyncLocal<ConcurrentDictionary<string, object>> _async_local = new AsyncLocal<ConcurrentDictionary<string, object>>();

    private AsyncLocal<ConcurrentDictionary<string, object>> _local => _async_local;

    public AsyncLocalRequestCache() { }

    void InitLocalCache()
    {
        if (_local.Value == null)
        {
            _local.Value = new ConcurrentDictionary<string, object>();
        }
    }

    public CacheResponse<T> GetObject<T>(string key)
    {
        InitLocalCache();

        if (_local.Value.TryGetValue(key, out var value))
        {
            if (value is T data)
            {
                return new CacheResponse<T>(data);
            }
        }
        return CacheResponse<T>.Empty;
    }

    public void RemoveKey(string key)
    {
        InitLocalCache();

        _local.Value.Remove(key, out var value);
    }

    public void SetObject<T>(string key, T data)
    {
        InitLocalCache();

        _local.Value[key] = data;
    }
}