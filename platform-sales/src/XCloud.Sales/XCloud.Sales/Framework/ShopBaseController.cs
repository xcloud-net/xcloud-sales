using XCloud.AspNetMvc.Controller;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Queue;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Configuration;
using XCloud.Sales.Service.Stores;

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

    protected SalesEventBusService SalesEventBusService =>
        this.LazyServiceProvider.LazyGetRequiredService<SalesEventBusService>();

    protected ICurrentStoreSelector CurrentStoreSelector =>
        this.LazyServiceProvider.LazyGetRequiredService<ICurrentStoreSelector>();
}