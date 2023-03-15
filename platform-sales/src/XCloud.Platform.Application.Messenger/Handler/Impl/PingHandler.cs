using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Constants;
using XCloud.Platform.Application.Messenger.Extension;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

[ExposeServices(typeof(IMessageHandler))]
public class PingHandler : IMessageHandler, IScopedDependency
{
    public string MessageType => MessageTypeConst.Ping;

    public int Sort => 1;

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        var info = context.Connection.ToRegInfo();
        await context.Connection.Server.RegistrationProvider.RegisterUserInfoAsync(info);

        context.Connection.ClientIdentity.PingTime = info.Payload.PingTimeUtc;

        await context.Connection.SendMessageToClientAsync(new MessageWrapper()
        {
            MessageType = MessageTypeConst.Ping,
            Data = "success"
        });
    }

    public Task HandleMessageFromTransportAsync(TransportMessageContext context)
    {
        throw new NotImplementedException();
    }
}