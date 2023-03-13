using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Common.Service.Messenger.RegistrationCenter;

public class UserRegistrationInfo : IEntityDto
{
    public string UserId { get; set; }
    public string DeviceType { get; set; }
    public UserRegistrationInfoPayload Payload { get; set; }
}

public class UserRegistrationInfoPayload : IEntityDto
{
    public string ServerInstanceId { get; set; }
    public DateTime PingTimeUtc { get; set; }
}

public class GroupRegistrationInfo : IEntityDto
{
    public string GroupId { get; set; }
    public string ServerInstance { get; set; }
    public GroupRegistrationInfoPayload Payload { get; set; }
}

public class GroupRegistrationInfoPayload : IEntityDto
{
    //
}