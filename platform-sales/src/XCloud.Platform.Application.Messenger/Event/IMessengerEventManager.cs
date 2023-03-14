using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Event;

public interface IMessengerEventManager : IMessengerEventSubscriber
{
    //
}

[ExposeServices(typeof(IMessengerEventManager))]
public class MessengerEventManager : IMessengerEventManager, IScopedDependency
{
    private readonly IMessengerEventSubscriber[] _subscribers;

    public MessengerEventManager(IServiceProvider serviceProvider)
    {
        this._subscribers = serviceProvider.GetServices<IMessengerEventSubscriber>().ToArray();
    }

    //
    public Task ServerStartedAsync(IMessengerServer server)
    {
        throw new NotImplementedException();
    }

    public Task ServerShutdownAsync(IMessengerServer server)
    {
        throw new NotImplementedException();
    }

    public Task MessageFromClientAsync(MessageWrapper messageWrapper)
    {
        throw new NotImplementedException();
    }

    public Task MessageFromRouterAsync(MessageWrapper messageWrapper)
    {
        throw new NotImplementedException();
    }

    public Task OnlineAsync(IConnection connection)
    {
        throw new NotImplementedException();
    }

    public Task OfflineAsync(IConnection connection)
    {
        throw new NotImplementedException();
    }
}