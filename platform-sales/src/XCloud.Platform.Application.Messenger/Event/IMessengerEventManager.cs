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
    private readonly ILogger _logger;

    public MessengerEventManager(IServiceProvider serviceProvider, ILogger<MessengerEventManager> logger)
    {
        this._subscribers = serviceProvider.GetServices<IMessengerEventSubscriber>().ToArray();
        this._logger = logger;
    }

    //
    public async Task ServerStartedAsync(IMessengerServer server)
    {
        foreach (var s in this._subscribers)
        {
            try
            {
                await s.ServerStartedAsync(server);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
                continue;
            }
        }
    }

    public async Task ServerShutdownAsync(IMessengerServer server)
    {
        foreach (var s in this._subscribers)
        {
            try
            {
                await s.ServerShutdownAsync(server);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
                continue;
            }
        }
    }

    public async Task MessageFromClientAsync(MessageDto messageDto)
    {
        foreach (var s in this._subscribers)
        {
            try
            {
                await s.MessageFromClientAsync(messageDto);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
                continue;
            }
        }
    }

    public async Task MessageFromRouterAsync(MessageDto messageDto)
    {
        foreach (var s in this._subscribers)
        {
            try
            {
                await s.MessageFromRouterAsync(messageDto);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
                continue;
            }
        }
    }

    public async Task OnlineAsync(IConnection connection)
    {
        foreach (var s in this._subscribers)
        {
            try
            {
                await s.OnlineAsync(connection);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
                continue;
            }
        }
    }

    public async Task OfflineAsync(IConnection connection)
    {
        foreach (var s in this._subscribers)
        {
            try
            {
                await s.OfflineAsync(connection);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
                continue;
            }
        }
    }
}