using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Extension;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

public class PingHandler : IMessageHandler
{
    public string MessageType => MessageTypeConst.Ping;

    public int Sort => 1;

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        var info = context.Connection.ToRegInfo();
        await context.Connection.Server.RegistrationProvider.RegisterUserInfoAsync(info);

        context.Connection.Client.PingTime = info.Payload.PingTimeUtc;

        await context.Connection.SendMessage(new MessageWrapper()
        {
            MessageType = MessageTypeConst.Ping,
            Payload = "success"
        });
    }

    public Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        throw new NotImplementedException();
    }
}