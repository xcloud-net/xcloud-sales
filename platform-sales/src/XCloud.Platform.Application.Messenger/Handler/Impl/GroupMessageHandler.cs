using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Constants;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

[ExposeServices(typeof(IMessageHandler))]
public class GroupMessageHandler : IMessageHandler, IScopedDependency
{
    public string MessageType => MessageTypeConst.Group;
    public int Sort => default;

    public Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        throw new NotImplementedException();
    }

    public Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        throw new NotImplementedException();
    }
}