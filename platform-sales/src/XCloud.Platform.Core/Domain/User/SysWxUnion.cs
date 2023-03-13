using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.User;

public class SysWxUnion : EntityBase, IMemberEntity
{
    public string Platform { get; set; }
    public string AppId { get; set; }
    public string OpenId { get; set; }
    public string UnionId { get; set; }
}