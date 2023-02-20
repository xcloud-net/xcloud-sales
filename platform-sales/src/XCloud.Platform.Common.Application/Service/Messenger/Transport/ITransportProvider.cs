using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Core.DataSerializer;
using XCloud.Platform.Common.Application.Service.Messenger.Connection;

namespace XCloud.Platform.Common.Application.Service.Messenger.Transport;

public interface ITransportProvider : IDisposable
{
    Task RouteToServerInstance(string instanceKey, MessageWrapper data);
    Task BroadCast(MessageWrapper data);
    Task SubscribeMessageEndpoint(string key, Func<MessageWrapper, Task> callback);
    Task SubscribeBroadcastMessageEndpoint(Func<MessageWrapper, Task> callback);
}

public class InMemoryTransportProvider : ITransportProvider, ISingletonDependency
{
    private readonly ILogger logger;
    private readonly IJsonDataSerializer messageSerializer;

    public InMemoryTransportProvider(IJsonDataSerializer messageSerializer, ILogger<InMemoryTransportProvider> logger)
    {
        this.messageSerializer = messageSerializer;
        this.logger = logger;
    }

    public async Task BroadCast(MessageWrapper data)
    {
        if (Subscribers != null)
            await Subscribers.Invoke(data);
    }

    public void Dispose()
    {
        //
    }

    public async Task RouteToServerInstance(string key, MessageWrapper data)
    {
        if (Subscribers != null)
            await Subscribers.Invoke(data);
    }

    private Func<MessageWrapper, Task> Subscribers = null;

    public async Task SubscribeBroadcastMessageEndpoint(Func<MessageWrapper, Task> callback)
    {
        this.Subscribers = callback;
        await Task.CompletedTask;
    }

    public async Task SubscribeMessageEndpoint(string key, Func<MessageWrapper, Task> callback)
    {
        this.Subscribers = callback;
        await Task.CompletedTask;
    }
}