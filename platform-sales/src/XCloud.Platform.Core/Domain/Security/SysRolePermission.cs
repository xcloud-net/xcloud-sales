using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Security;

public class SysRolePermission : EntityBase, IMemberEntity
{
    public string PermissionKey { get; set; }

    public string RoleId { get; set; }
}