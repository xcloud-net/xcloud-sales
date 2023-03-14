using XCloud.Platform.Application.Messenger.Constants;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

public class GroupMessageHandler : IMessageHandler
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