using Volo.Abp.Authorization.Permissions;
using Volo.Abp.MultiTenancy;

namespace XCloud.Platform.Shared;

public class MemberShipPermissionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup("member-ship");

        group.AddPermission("manage-tenant", multiTenancySide: MultiTenancySides.Host);
        group.AddPermission("manage-user", multiTenancySide: MultiTenancySides.Host);
        group.AddPermission("manage-role", multiTenancySide: MultiTenancySides.Host);
        group.AddPermission("manage-department", multiTenancySide: MultiTenancySides.Host);
        group.AddPermission("manage-dashboard", multiTenancySide: MultiTenancySides.Host);
    }
}