using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Constants;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

[ExposeServices(typeof(IMessageHandler))]
public class BroadcastHandler : IMessageHandler, IScopedDependency
{
    public string MessageType => MessageTypeConst.BroadCast;

    public int Sort => 1;

    public async Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        var tasks = context.MessengerServer.ConnectionManager.AsReadOnlyList()
            .Select(x => x.SendMessageToClientAsync(context.Message)).ToArray();
        await Task.WhenAll(tasks);
    }

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        throw new NotImplementedException();
    }
}