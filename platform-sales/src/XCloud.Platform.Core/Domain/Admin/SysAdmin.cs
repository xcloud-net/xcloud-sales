using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared.Entity;

namespace XCloud.Platform.Core.Domain.Admin;

public class SysAdmin : EntityBase, IMemberEntity, IHasUserId
{
    public string UserId { get; set; }

    public bool IsActive { get; set; }

    public bool IsSuperAdmin { get; set; }
}