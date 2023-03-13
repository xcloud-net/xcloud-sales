using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Security;

public class SysPermission : EntityBase, IMemberEntity
{
    public string PermissionKey { get; set; }
    public string Description { get; set; }
    public string Group { get; set; }
    public string AppKey { get; set; }
}