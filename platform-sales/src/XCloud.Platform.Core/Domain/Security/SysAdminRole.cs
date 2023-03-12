using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Security;

/// <summary>
/// 用户角色关联
/// </summary>
public class SysAdminRole : EntityBase, IMemberEntity
{
    public string RoleId { get; set; }

    public string AdminId { get; set; }
}