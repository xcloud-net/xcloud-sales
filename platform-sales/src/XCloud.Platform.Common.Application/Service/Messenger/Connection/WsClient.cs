using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Common.Application.Service.Messenger.Connection;

public class WsClient : IEntityDto
{
    public WsClient() { }
    public string SubjectId { get; set; }
    public string DeviceType { get; set; }
    public string ConnectionId { get; set; }

    public DateTime PingTime { get; set; }
}