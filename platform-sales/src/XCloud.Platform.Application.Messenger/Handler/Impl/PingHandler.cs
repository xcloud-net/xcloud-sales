using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using XCloud.Platform.Application.Messenger.Constants;
using XCloud.Platform.Application.Messenger.Extension;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Handler.Impl;

[ExposeServices(typeof(IMessageHandler))]
public class PingHandler : IMessageHandler, IScopedDependency
{
    private readonly IClock _clock;

    public PingHandler(IClock clock)
    {
        _clock = clock;
    }

    public string MessageType => MessageTypeConst.Ping;

    public int Sort => 1;

    public async Task HandleMessageFromClientAsync(ClientMessageContext context)
    {
        var info = context.Connection.ToRegInfo();
        info.PingTime = this._clock.Now;
        
        await context.Connection.Server.UserRegistrationService.RegisterUserInfoAsync(info);

        context.Connection.ClientIdentity.PingTime = info.PingTime;

        await context.Connection.SendMessageToClientAsync(new MessageDto()
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