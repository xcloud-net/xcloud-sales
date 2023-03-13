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
    
    protected CommonMessageBus CommonServiceMessageBus =>
        this.LazyServiceProvider.LazyGetRequiredService<CommonMessageBus>();

    protected MemberMessageBus MemberShipMessageBus =>
        this.LazyServiceProvider.LazyGetRequiredService<MemberMessageBus>();
    
    protected IAdminSecurityService SecurityService =>
        this.LazyServiceProvider.LazyGetRequiredService<IAdminSecurityService>();
}