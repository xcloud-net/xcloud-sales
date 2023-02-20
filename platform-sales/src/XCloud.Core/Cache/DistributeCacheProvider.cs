using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;

namespace XCloud.Core.Cache;

public class DistributeCacheProvider : ICacheProvider
{
    private readonly IDistributedCache _distributedCache;
    private readonly CacheSerializer _dataSerializer;
    private readonly CacheProviderOption _option;

    public DistributeCacheProvider(IConfiguration configuration,
        IDistributedCache distributedCache,
        CacheSerializer dataSerializer,
        ILogger<DistributeCacheProvider> logger)
    {
        this._distributedCache = distributedCache;
        this._dataSerializer = dataSerializer;
        this.Logger = logger;
        this._option = new CacheProviderOption(configuration);
    }

    public ILogger Logger { get; }

    public async Task<CacheResponse<T>> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        
        var json = await this._distributedCache.GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(json))
        {
            return CacheResponse<T>.Empty;
        }

        var res = this._dataSerializer.DeserializeCacheResultFromString<T>(json);
        return res;
    }

    private DistributedCacheEntryOptions BuildOptions(TimeSpan expire)
    {
        var option = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expire
        };
        return option;
    }

    public async Task SetAsync<T>(string key, T data, TimeSpan? expire)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        var json = this._dataSerializer.SerializeCacheResultToString(data);

        if (expire != null)
            await this._distributedCache.SetStringAsync(key, json, this.BuildOptions(expire.Value));
        else
            await this._distributedCache.SetStringAsync(key, json);
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        await this._distributedCache.RemoveAsync(key);
    }
}