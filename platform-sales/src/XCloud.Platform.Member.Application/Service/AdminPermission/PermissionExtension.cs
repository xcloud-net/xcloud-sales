using System.Collections.Generic;

namespace XCloud.Platform.Member.Application.Service.AdminPermission;

public static class PermissionExtension
{
    public static string[] GetAllPermissions(this GrantedPermissionResponse dto)
    {
        var permissions = new List<string>();

        permissions.AddManyItems(dto.Permissions);

        if (dto.RecalledPermissions?.Any() ?? false)
        {
            permissions = permissions.Except(dto.RecalledPermissions).ToList();
        }

        permissions = permissions.Distinct().ToList();

        return permissions.ToArray();
    }
}