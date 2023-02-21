using Microsoft.AspNetCore.Http;
using XCloud.Core.Cache.RequestCache;
using XCloud.Sales.Service.Authentication;

namespace XCloud.Sales.Framework;

/// <summary>
/// Work context
/// </summary>
public interface ISalesWorkContext
{
    AuthedStoreAdministratorResult AuthedStoreAdministrator { get; set; }

    AuthedStoreUserResult AuthedStoreUser { get; set; }

    AuthedGlobalUserResult AuthedGlobalUser { get; set; }

    AuthedStoreManagerResult AuthedStoreManager { get; set; }
}
    
/// <summary>
/// Working context for web application
/// </summary>
[ExposeServices(typeof(ISalesWorkContext))]
public class WebWorkContext : ISalesWorkContext, ITransientDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestCacheProvider _requestCacheProvider;

    public WebWorkContext(IHttpContextAccessor httpContextAccessor,
        IRequestCacheProvider requestCacheProvider)
    {
        this._requestCacheProvider = requestCacheProvider;
        this._httpContextAccessor = httpContextAccessor;
    }

    private void SetHttpContextItems<T>(string key, T data)
    {
        this._requestCacheProvider.SetObject(key, data);
    }

    private T GetFromHttpContextItems<T>(string key)
    {
        var dataInCache = this._requestCacheProvider.GetObject<T>(key);

        return dataInCache.Data;
    }

    public AuthedStoreUserResult AuthedStoreUser
    {
        get => this.GetFromHttpContextItems<AuthedStoreUserResult>(nameof(AuthedStoreUser));
        set => this.SetHttpContextItems(nameof(AuthedStoreUser), value);
    }

    public AuthedStoreManagerResult AuthedStoreManager
    {
        get => this.GetFromHttpContextItems<AuthedStoreManagerResult>(nameof(AuthedStoreManager));
        set => this.SetHttpContextItems(nameof(AuthedStoreManager), value);
    }

    public AuthedStoreAdministratorResult AuthedStoreAdministrator
    {
        get => this.GetFromHttpContextItems<AuthedStoreAdministratorResult>(nameof(AuthedStoreAdministrator));
        set => this.SetHttpContextItems(nameof(AuthedStoreAdministrator), value);
    }

    public AuthedGlobalUserResult AuthedGlobalUser
    {
        get => this.GetFromHttpContextItems<AuthedGlobalUserResult>(nameof(AuthedGlobalUser));
        set => this.SetHttpContextItems(nameof(AuthedGlobalUser), value);
    }
}