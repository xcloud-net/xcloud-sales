using System;
using Microsoft.AspNetCore.Http;
using XCloud.Core.Cache;
using XCloud.Core.Cache.RequestCache;

namespace XCloud.AspNetMvc.RequestCache;

public class HttpContextItemRequestCache : IRequestCacheProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextItemRequestCache(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext
    {
        get
        {
            if (this._httpContextAccessor.HttpContext == null)
                throw new NotSupportedException(nameof(this.HttpContext));
            return this._httpContextAccessor.HttpContext;
        }
    }

    public CacheResponse<T> GetObject<T>(string key)
    {
        if (HttpContext.Items[key] is T d)
        {
            return new CacheResponse<T>(d);
        }
        else
        {
            return CacheResponse<T>.Empty;
        }
    }

    public void RemoveKey(string key)
    {
        HttpContext.Items.Remove(key);
    }

    public void SetObject<T>(string key, T data)
    {
        HttpContext.Items[key] = data;
    }
}