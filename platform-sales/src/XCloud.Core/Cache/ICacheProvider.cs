using System.Threading.Tasks;

namespace XCloud.Core.Cache;

public interface ICacheProvider
{
    ILogger Logger { get; }
    
    Task RemoveAsync(string key);
    
    Task<CacheResponse<T>> GetAsync<T>(string key);
    
    Task SetAsync<T>(string key, T data, TimeSpan? expire);
}