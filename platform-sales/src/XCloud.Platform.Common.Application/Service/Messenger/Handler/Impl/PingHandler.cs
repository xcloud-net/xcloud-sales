using System.Threading.Tasks;
using XCloud.Platform.Common.Application.Service.Messenger.Connection;

namespace XCloud.Platform.Common.Application.Service.Messenger.Handler.Impl;

public class PingHandler : IMessageHandler
{
    public string MessageType => MessageTypeConst.PING;

    public int Sort => 1;

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        var info = context.Connection.ToRegInfo();
        await context.Connection.Server.RegistrationProvider.RegisterUserInfoAsync(info);

        context.Connection.Client.PingTime = info.Payload.PingTimeUtc;

        await context.Connection.SendMessage(new MessageWrapper()
        {
            MessageType = MessageTypeConst.PING,
            Payload = "success"
        });
    }

    public Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        throw new NotImplementedException();
    }
}