using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Messenger.Client;

public class ClientIdentity : IEntityDto
{
    public ClientIdentity() { }
    
    public string SubjectId { get; set; }
    public string DeviceType { get; set; }
    public string ConnectionId { get; set; }

    public DateTime PingTime { get; set; }
}