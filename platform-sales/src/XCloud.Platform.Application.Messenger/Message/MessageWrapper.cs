using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace XCloud.Platform.Application.Messenger.Message;

public class MessageWrapper : IEntityDto, IHasCreationTime
{
    public string MessageType { get; set; }

    public string Body { get; set; }

    public DateTime CreationTime { get; set; }
}