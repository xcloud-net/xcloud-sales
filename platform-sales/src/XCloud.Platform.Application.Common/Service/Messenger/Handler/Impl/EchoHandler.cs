using System.Threading.Tasks;
using XCloud.Platform.Application.Common.Service.Messenger.Connection;

namespace XCloud.Platform.Application.Common.Service.Messenger.Handler.Impl;

public class EchoHandler : IMessageHandler
{
    public string MessageType => MessageTypeConst.ECHO;

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