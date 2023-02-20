using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.AdminPermission;

/// <summary>
/// 用户角色关联
/// </summary>
public class SysAdminRole : EntityBase, IMemberEntity
{
    public string RoleId { get; set; }

    public string AdminId { get; set; }
}