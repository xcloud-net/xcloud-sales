using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using XCloud.Application.Service;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Shared.Localization;

namespace XCloud.Platform.Application.Member.Service.Security;

public interface IAbpPermissionService: IXCloudApplicationService
{
    Task<string[]> QueryAllPermissionsAsync();
    
    Task<AntDesignTreeNode[]> BuildAntDesignTreeAsync();
}

public class AbpPermissionService : PlatformApplicationService, IAbpPermissionService
{
    private readonly IPermissionDefinitionManager _permissionManager;

    public AbpPermissionService(IPermissionDefinitionManager permissionService)
    {
        _permissionManager = permissionService;
    }

    public async Task<string[]> QueryAllPermissionsAsync()
    {
        await Task.CompletedTask;

        var res = _permissionManager.GetPermissions().Select(x => x.Name).ToArray();
        return res;
    }

    public async Task<AntDesignTreeNode[]> BuildAntDesignTreeAsync()
    {
        await Task.CompletedTask;
        LocalizationResource = typeof(PlatformResource);

        var list = new List<string>();
        AntDesignTreeNode PermissionDefinition(PermissionDefinition m)
        {
            list.AddOnceOrThrow(m.Name);

            return new AntDesignTreeNode()
            {
                key = m.Name,
                title = L[m.Name]?.Value ?? m.Name,
                raw_data = new { },
                children = m.Children.Select(PermissionDefinition).ToArray()
            };
        }

        AntDesignTreeNode GroupDefinition(PermissionGroupDefinition x)
        {
            list.AddOnceOrThrow(x.Name);

            return new AntDesignTreeNode()
            {
                key = x.Name,
                title = L[x.Name]?.Value ?? x.Name,
                raw_data = new { },
                children = x.Permissions.Select(PermissionDefinition).ToArray()
            };
        }

        var groups = _permissionManager.GetGroups();

        var res = groups.Select(GroupDefinition).ToArray();

        return res;
    }
}