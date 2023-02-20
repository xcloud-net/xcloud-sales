using Volo.Abp.Application.Services;
using XCloud.Core.Application;
using XCloud.Redis.DistributedLock;
using XCloud.Sales.Queue;
using XCloud.Sales.Services.Stores;

namespace XCloud.Sales.Services;

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

    protected IEventBusService EventBusService => LazyServiceProvider.LazyGetRequiredService<IEventBusService>();
}