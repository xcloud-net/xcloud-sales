using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Security;

public class SysRole : EntityBase, IMemberEntity
{
    public string Name { get; set; }

    public string Description { get; set; }
}