using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Notification;

public class SysNotification : EntityBase, IPlatformEntity
{
    public string Title { get; set; }

    public string Content { get; set; }

    public string App { get; set; }
    
    public bool Read { get; set; }
    
    public DateTime? ReadTime { get; set; }

    public string UserId { get; set; }

    public string ActionType { get; set; }
    
    public string Data { get; set; }

    public string SenderId { get; set; }

    public string SenderType { get; set; }
}