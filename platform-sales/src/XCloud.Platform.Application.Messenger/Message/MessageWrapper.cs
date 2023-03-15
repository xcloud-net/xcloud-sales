using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.ObjectExtending;

namespace XCloud.Platform.Application.Messenger.Message;

public class MessageWrapper : ExtensibleObject, IEntityDto<string>, IHasCreationTime
{
    public string Id { get; set; }

    public string Message { get; set; }

    public string MessageType { get; set; }

    public string Data { get; set; }

    public DateTime CreationTime { get; set; }
}