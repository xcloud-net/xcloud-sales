using XCloud.AspNetMvc.Controller;
using XCloud.Platform.Common.Application.Queue;
using XCloud.Platform.Member.Application.Queue;
using XCloud.Platform.Member.Application.Service.AdminPermission;

namespace XCloud.Platform.Framework.Controller;

public abstract class PlatformBaseController : XCloudBaseController
{
    protected ICommonServiceMessageBus CommonServiceMessageBus =>
        this.LazyServiceProvider.LazyGetRequiredService<ICommonServiceMessageBus>();

    protected IMemberShipMessageBus MemberShipMessageBus =>
        this.LazyServiceProvider.LazyGetRequiredService<IMemberShipMessageBus>();
    
    protected IAdminPermissionService PermissionService =>
        this.LazyServiceProvider.LazyGetRequiredService<IAdminPermissionService>();
}