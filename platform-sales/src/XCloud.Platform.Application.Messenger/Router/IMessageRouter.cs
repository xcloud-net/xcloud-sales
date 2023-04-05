using Volo.Abp.DependencyInjection;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Router;

public interface IMessageRouter : IDisposable
{
    Task RouteToServerInstance(string instanceKey, MessageDto data);

    Task SubscribeMessageEndpoint(string instanceKey, Func<MessageDto, Task> callback);
}

public class InMemoryMessageRouter : IMessageRouter, ISingletonDependency
{
    private readonly ILogger _logger;
    private readonly IJsonDataSerializer _messageSerializer;

    public InMemoryMessageRouter(IJsonDataSerializer messageSerializer, ILogger<InMemoryMessageRouter> logger)
    {
        this._messageSerializer = messageSerializer;
        this._logger = logger;
    }

    public void Dispose()
    {
        //
    }

    public async Task RouteToServerInstance(string key, MessageDto data)
    {
        if (_subscribers != null)
            await _subscribers.Invoke(data);
    }

    private Func<MessageDto, Task> _subscribers = null;

    public async Task SubscribeMessageEndpoint(string key, Func<MessageDto, Task> callback)
    {
        this._subscribers = callback;
        await Task.CompletedTask;
    }
}