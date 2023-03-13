using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

public class EchoHandler : IMessageHandler
{
    public string MessageType => MessageTypeConst.Echo;

    public int Sort => 1;

    public Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        throw new NotImplementedException();
    }

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        await context.Connection.SendMessage(context.Message);
    }
}