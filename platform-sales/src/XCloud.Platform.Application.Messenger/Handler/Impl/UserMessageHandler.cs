using Volo.Abp.Application.Dtos;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

public class UserToUserPayload : IEntityDto
{
    public UserToUserPayload()
    {
        //
    }

    public string Sender { get; set; }
    public string Reciever { get; set; }
    public string Message { get; set; }
    public DateTime SendTime { get; set; }
}

public class UserMessageHandler : IMessageHandler
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
            context.WsServer.MessageSerializer.DeserializeFromString<UserToUserPayload>(context.Message.Payload);

        var find = false;

        foreach (var connection in context.WsServer.ClientManager.AllConnections())
        {
            if (connection.Client.SubjectId == payload.Reciever)
            {
                find = true;
                await connection.SendMessage(context.Message);
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
                context.Message.Payload);
        var server_instance_id =
            await context.Connection.Server.RegistrationProvider.GetUserServerInstancesAsync(payload.Reciever);

        foreach (var serverId in server_instance_id)
        {
            await context.WsServer.TransportProvider.RouteToServerInstance(serverId, context.Message);
        }
    }
}