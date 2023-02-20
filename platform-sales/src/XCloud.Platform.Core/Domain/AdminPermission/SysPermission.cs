using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.AdminPermission;

public class SysPermission : EntityBase, IMemberEntity
{
    public string PermissionKey { get; set; }
    public string Description { get; set; }
    public string Group { get; set; }
    public string AppKey { get; set; }
}