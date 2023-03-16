using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Messenger;

public class SysMessage : EntityBase, IPlatformEntity
{
    public string Message { get; set; }
    
    public string MessageType { get; set; }
    
    public string Data { get; set; }
}