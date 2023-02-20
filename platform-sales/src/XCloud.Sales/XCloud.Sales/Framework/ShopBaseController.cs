using XCloud.AspNetMvc.Controller;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Queue;
using XCloud.Sales.Services.Authentication;
using XCloud.Sales.Services.Configuration;
using XCloud.Sales.Services.Stores;

namespace XCloud.Sales.Framework;

public abstract class ShopBaseController : XCloudBaseController
{
    protected IMallSettingService MallSettingService =>
        this.LazyServiceProvider.LazyGetRequiredService<IMallSettingService>();

    protected PlatformInternalService PlatformInternalService =>
        this.LazyServiceProvider.LazyGetRequiredService<PlatformInternalService>();

    protected IStoreAuthService StoreAuthService =>
        this.LazyServiceProvider.LazyGetRequiredService<IStoreAuthService>();

    protected ISalesPermissionService SalesPermissionService =>
        this.LazyServiceProvider.LazyGetRequiredService<ISalesPermissionService>();

    protected IEventBusService EventBusService =>
        this.LazyServiceProvider.LazyGetRequiredService<IEventBusService>();

    protected ICurrentStoreSelector CurrentStoreSelector =>
        this.LazyServiceProvider.LazyGetRequiredService<ICurrentStoreSelector>();
}