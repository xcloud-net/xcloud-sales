using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Common.Service.Messenger.Connection;

public static class MessageTypeConst
{
    public const string USER_TO_USER = "user-to-user";
    
    public const string ECHO = "echo";
    
    public const string PING = "ping";
    
    public const string BROAD_CAST = "broad-cast";
}

public class MessageWrapper : IEntityDto
{
    public string MessageType { get; set; }
    
    public string Payload { get; set; }
    
    public DateTime MessageTime { get; set; }
}