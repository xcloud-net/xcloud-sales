namespace XCloud.Core.Cache.RequestCache;

public interface IRequestCacheProvider
{
    CacheResponse<T> GetObject<T>(string key);
     
    void SetObject<T>(string key, T data);
        
    void RemoveKey(string key);
}