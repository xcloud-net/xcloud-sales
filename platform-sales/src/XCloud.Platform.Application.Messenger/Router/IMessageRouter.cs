using Volo.Abp.DependencyInjection;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Message;

namespace XCloud.Platform.Application.Messenger.Router;

public interface IMessageRouter : IDisposable
{
    Task RouteToServerInstance(string instanceKey, MessageWrapper data);
    
    Task BroadCast(MessageWrapper data);
    
    Task SubscribeMessageEndpoint(string instanceKey, Func<MessageWrapper, Task> callback);
    
    Task SubscribeBroadcastMessageEndpoint(Func<MessageWrapper, Task> callback);
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

    public async Task BroadCast(MessageWrapper data)
    {
        if (_subscribers != null)
            await _subscribers.Invoke(data);
    }

    public void Dispose()
    {
        //
    }

    public async Task RouteToServerInstance(string key, MessageWrapper data)
    {
        if (_subscribers != null)
            await _subscribers.Invoke(data);
    }

    private Func<MessageWrapper, Task> _subscribers = null;

    public async Task SubscribeBroadcastMessageEndpoint(Func<MessageWrapper, Task> callback)
    {
        this._subscribers = callback;
        await Task.CompletedTask;
    }

    public async Task SubscribeMessageEndpoint(string key, Func<MessageWrapper, Task> callback)
    {
        this._subscribers = callback;
        await Task.CompletedTask;
    }
}