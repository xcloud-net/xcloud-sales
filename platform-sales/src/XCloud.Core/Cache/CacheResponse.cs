using XCloud.Core.Dto;

namespace XCloud.Core.Cache;

[Serializable]
public class CacheResponse<T> : ApiResponse<T>
{
    static CacheResponse<T> BuildEmpty()
    {
        var empty = new CacheResponse<T>();
        empty.SetError("empty result from cache");
        return empty;
    }

    /// <summary>
    /// empty result from cache
    /// </summary>
    public static CacheResponse<T> Empty => BuildEmpty();

    public CacheResponse() { }

    public CacheResponse(T data) : base(data) { }
}