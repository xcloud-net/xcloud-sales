using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.AdminPermission;

public class SysRole : EntityBase, IMemberEntity
{
    public string Name { get; set; }

    public string Description { get; set; }
}