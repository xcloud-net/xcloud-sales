using XCloud.Core.Application;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared;

namespace XCloud.Platform.Core.Domain.Admin;

public class SysAdmin : EntityBase, IMemberEntity, IHasUserId
{
    public string UserId { get; set; }

    public bool IsActive { get; set; }

    public bool IsSuperAdmin { get; set; }
}