using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;
using XCloud.Application.ServiceDiscovery;
using XCloud.Core.Cache;
using XCloud.Core.Json;

namespace XCloud.Application.Service;

public interface IXCloudApplicationService : IApplicationService
{
    //
}

public abstract class XCloudApplicationService : ApplicationService
{
    protected IConfiguration Configuration => this.LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    protected ICacheProvider CacheProvider => this.LazyServiceProvider.LazyGetRequiredService<ICacheProvider>();

    protected IMemoryCache MemoryCache => this.LazyServiceProvider.LazyGetRequiredService<IMemoryCache>();
    
    protected IJsonDataSerializer JsonDataSerializer =>
        this.LazyServiceProvider.LazyGetRequiredService<IJsonDataSerializer>();

    protected IObjectValidator ObjectValidator =>
        this.LazyServiceProvider.LazyGetRequiredService<IObjectValidator>();

    protected IServiceDiscoveryService ServiceDiscoveryService =>
        this.LazyServiceProvider.LazyGetRequiredService<IServiceDiscoveryService>();
}