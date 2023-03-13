using XCloud.Application.ServiceDiscovery;
using XCloud.AspNetMvc.Controller;
using XCloud.Platform.Application.Common.Queue;
using XCloud.Platform.Application.Member.Queue;
using XCloud.Platform.Application.Member.Service.Security;

namespace XCloud.Platform.Framework.Controller;

public abstract class PlatformBaseController : XCloudBaseController
{
    protected IServiceDiscoveryService ServiceDiscoveryService =>
        this.LazyServiceProvider.LazyGetRequiredService<IServiceDiscoveryService>();
    
    protected ICommonServiceMessageBus CommonServiceMessageBus =>
        this.LazyServiceProvider.LazyGetRequiredService<ICommonServiceMessageBus>();

    protected IMemberShipMessageBus MemberShipMessageBus =>
        this.LazyServiceProvider.LazyGetRequiredService<IMemberShipMessageBus>();
    
    protected IAdminSecurityService SecurityService =>
        this.LazyServiceProvider.LazyGetRequiredService<IAdminSecurityService>();
}