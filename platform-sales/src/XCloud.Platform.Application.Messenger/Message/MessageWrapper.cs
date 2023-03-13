using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Messenger.Message;

public static class MessageTypeConst
{
    public const string UserToUser = "user-to-user";
    
    public const string Echo = "echo";
    
    public const string Ping = "ping";
    
    public const string BroadCast = "broad-cast";
}

public class MessageWrapper : IEntityDto
{
    public string MessageType { get; set; }
    
    public string Payload { get; set; }
    
    public DateTime MessageTime { get; set; }
}