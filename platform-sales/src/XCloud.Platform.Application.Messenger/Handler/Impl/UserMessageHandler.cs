using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Constants;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

public class UserToUserPayload : IEntityDto<string>
{
    public UserToUserPayload()
    {
        //
    }

    public string Id { get; set; }

    public string Sender { get; set; }
    public string Receiver { get; set; }

    public string Message { get; set; }

    public DateTime SendTime { get; set; }
}

[ExposeServices(typeof(IMessageHandler))]
public class UserMessageHandler : IMessageHandler, IScopedDependency
{
    private readonly ILogger _logger;

    public UserMessageHandler(ILogger<UserMessageHandler> logger)
    {
        this._logger = logger;
    }

    public string MessageType => MessageTypeConst.UserToUser;

    public int Sort => 1;

    public async Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        var payload =
            context.MessengerServer.MessageSerializer.DeserializeFromString<UserToUserPayload>(context.Message.Data);

        var find = false;

        foreach (var connection in context.MessengerServer.ConnectionManager.AsReadOnlyList())
        {
            if (connection.ClientIdentity.SubjectId == payload.Receiver)
            {
                find = true;
                await connection.SendMessageToClientAsync(context.Message);
            }
        }

        if (!find)
        {
            //消息路由过来但是本地没有对应的连接
            this._logger.LogInformation($"destination not found");
        }
    }

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        var payload =
            context.Connection.Server.MessageSerializer.DeserializeFromString<UserToUserPayload>(
                context.Message.Data);
        var server_instance_id =
            await context.Connection.Server.RegistrationProvider.GetUserServerInstancesAsync(payload.Receiver);

        foreach (var serverId in server_instance_id)
        {
            await context.MessengerServer.MessageRouter.RouteToServerInstance(serverId, context.Message);
        }
    }
}