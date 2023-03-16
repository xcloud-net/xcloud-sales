using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Messenger;

public class SysUserOnlineStatus : EntityBase, IPlatformEntity
{
    public string UserId { get; set; }
    public string DeviceId { get; set; }
    public string ServerInstanceId { get; set; }
    public DateTime PingTime { get; set; }
}