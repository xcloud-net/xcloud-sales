using Volo.Abp.DependencyInjection;
using XCloud.Core.DataSerializer;
using XCloud.Core.Dto;

namespace XCloud.Core.Cache;

[ExposeServices(typeof(CacheSerializer))]
public class CacheSerializer : IScopedDependency
{
    private readonly IJsonDataSerializer _jsonDataSerializer;
    public CacheSerializer(IJsonDataSerializer jsonDataSerializer)
    {
        this._jsonDataSerializer = jsonDataSerializer;
    }

    public CacheResponse<T> DeserializeCacheResultFromString<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json));
        
        var res = this._jsonDataSerializer.DeserializeFromString<CacheResponse<T>>(json);
        if (res == null)
            throw new CacheException("缓存中的数据无法转换成entity，转换后为null");

        res.ResetError();

        return res;
    }

    public string SerializeCacheResultToString<T>(T data)
    {
        var cacheObject = new CacheResponse<T>(data);
        
        var res = this._jsonDataSerializer.SerializeToString(cacheObject);
        
        return res;
    }
}