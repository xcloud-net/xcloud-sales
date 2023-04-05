using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Constants;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

[ExposeServices(typeof(IMessageHandler))]
public class BroadcastHandler : IMessageHandler, IScopedDependency
{
    private readonly IServerInstanceService _serverInstanceService;

    public BroadcastHandler(IServerInstanceService serverInstanceService)
    {
        _serverInstanceService = serverInstanceService;
    }

    public string MessageType => MessageTypeConst.BroadCast;

    public int Sort => 1;

    public async Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        var connections = context.MessengerServer.ConnectionManager.AsReadOnlyList();

        foreach (var connection in connections)
        {
            await connection.SendMessageToClientAsync(context.Message);
        }
    }

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        var instances = await this._serverInstanceService.QueryAllInstancesAsync();

        foreach (var instanceId in instances)
        {
            await context.MessengerServer.MessageRouter.RouteToServerInstance(instanceId, context.Message);
        }
    }
}