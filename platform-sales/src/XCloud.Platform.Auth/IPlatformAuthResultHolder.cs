using Volo.Abp.DependencyInjection;

using XCloud.Core.Cache.RequestCache;
using XCloud.Platform.Application.Member.Service.Admin;
using XCloud.Platform.Application.Member.Service.User;

namespace XCloud.Platform.Auth;

/// <summary>
/// Work context
/// </summary>
public interface IPlatformAuthResultHolder
{
    UserAuthResponse AuthSysUser { get; set; }

    AdminAuthResponse AuthSysAdmin { get; set; }
}

/// <summary>
/// Working context for web application
/// </summary>
[ExposeServices(typeof(IPlatformAuthResultHolder))]
public class PlatformAuthResultHolder : IPlatformAuthResultHolder, IScopedDependency
{
    private readonly IRequestCacheProvider _requestCacheProvider;

    public PlatformAuthResultHolder(IRequestCacheProvider requestCacheProvider)
    {
        this._requestCacheProvider = requestCacheProvider;
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

    public UserAuthResponse AuthSysUser
    {
        get => this.GetFromHttpContextItems<UserAuthResponse>(nameof(AuthSysUser));
        set => this.SetHttpContextItems(nameof(AuthSysUser), value);
    }

    public AdminAuthResponse AuthSysAdmin
    {
        get => this.GetFromHttpContextItems<AdminAuthResponse>(nameof(AuthSysAdmin));
        set => this.SetHttpContextItems(nameof(AuthSysAdmin), value);
    }
}