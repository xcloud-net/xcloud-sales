using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Event;

public interface IMessengerEventSubscriber
{
    Task ServerStartedAsync(IMessengerServer server);
    Task ServerShutdownAsync(IMessengerServer server);

    Task MessageFromClientAsync(MessageDto messageDto);
    Task MessageFromRouterAsync(MessageDto messageDto);

    Task OnlineAsync(IConnection connection);
    Task OfflineAsync(IConnection connection);
}