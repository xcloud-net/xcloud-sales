using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Messenger.Registry;

public class UserRegistrationInfo : IEntityDto
{
    public string UserId { get; set; }
    
    public string DeviceType { get; set; }
    
    public string ServerInstanceId { get; set; }
    
    public DateTime PingTime { get; set; }

    public UserRegistrationInfoPayload Payload { get; set; }
}

[Obsolete]
public class UserRegistrationInfoPayload : IEntityDto
{
    public string ServerInstanceId { get; set; }
    
    public DateTime PingTimeUtc { get; set; }
}