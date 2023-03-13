using System.Threading.Tasks;
using XCloud.Platform.Application.Common.Service.Messenger.Connection;

namespace XCloud.Platform.Application.Common.Service.Messenger.Handler.Impl;

public class BroadcastHandler : IMessageHandler
{
    public string MessageType => MessageTypeConst.BROAD_CAST;

    public int Sort => 1;

    public async Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        var tasks = context.WsServer.ClientManager.AllConnections().Select(x => x.SendMessage(context.Message)).ToArray();
        await Task.WhenAll(tasks);
    }

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        await context.WsServer.TransportProvider.BroadCast(context.Message);
    }
}