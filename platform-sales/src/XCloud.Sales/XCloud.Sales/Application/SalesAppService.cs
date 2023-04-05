using Volo.Abp.Application.Services;
using XCloud.Application.Service;
using XCloud.Redis.DistributedLock;
using XCloud.Sales.Cache;
using XCloud.Sales.Queue;
using XCloud.Sales.Service.Stores;

namespace XCloud.Sales.Application;

public interface ISalesAppService : IApplicationService
{
    //
}

public abstract class SalesAppService : XCloudApplicationService, ISalesAppService
{
    protected ISalesCacheKeyManager SalesCacheKeyManager =>
        this.LazyServiceProvider.LazyGetRequiredService<ISalesCacheKeyManager>();

    protected RedLockClient RedLockClient => LazyServiceProvider.LazyGetRequiredService<RedLockClient>();

    protected ICurrentStoreSelector CurrentStoreSelector =>
        this.LazyServiceProvider.LazyGetRequiredService<ICurrentStoreSelector>();

    protected SalesEventBusService SalesEventBusService => LazyServiceProvider.LazyGetRequiredService<SalesEventBusService>();
}