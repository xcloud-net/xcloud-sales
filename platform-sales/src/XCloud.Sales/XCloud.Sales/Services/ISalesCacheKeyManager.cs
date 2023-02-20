using Microsoft.Extensions.Configuration;

namespace XCloud.Sales.Services;

public interface ISalesCacheKeyManager
{
    string Prefix { get; }
    string MallSettingsKey(string key);
}

[ExposeServices(typeof(ISalesCacheKeyManager))]
public class SalesCacheKeyManager : ISalesCacheKeyManager, ITransientDependency
{
    public string Prefix => "mall";

    private readonly ICacheProvider _cacheProvider;
    private readonly IConfiguration _configuration;

    public SalesCacheKeyManager(IConfiguration configuration, ICacheProvider cacheProvider)
    {
        this._configuration = configuration;
        this._cacheProvider = cacheProvider;
    }

    public string MallSettingsKey(string key) => $"{this.Prefix}-mall-settings-{key}";
}