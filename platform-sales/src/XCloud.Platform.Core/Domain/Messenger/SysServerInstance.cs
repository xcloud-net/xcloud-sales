using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Messenger;

public class SysServerInstance : EntityBase, IPlatformEntity
{
    public string InstanceId { get; set; }
    public int ConnectionCount { get; set; }
    public DateTime PingTime { get; set; }
}