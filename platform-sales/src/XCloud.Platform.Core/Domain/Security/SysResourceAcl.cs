using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Security;

/// <summary>
/// 数据权限
/// </summary>
public class SysResourceAcl : EntityBase, IMemberEntity
{
    /// <summary>
    /// 按钮、接口、模块、功能
    /// </summary>
    public string ResourceType { get; set; }
    public string ResourceId { get; set; }

    /// <summary>
    /// 权限、角色、部门
    /// </summary>
    public string PermissionType { get; set; }
    public string PermissionId { get; set; }

    /// <summary>
    /// allow、deny
    /// </summary>
    public int AccessControlType { get; set; }
}