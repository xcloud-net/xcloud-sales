using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Event.Impl;

[ExposeServices(typeof(IMessengerEventSubscriber))]
public class MessageStoreSubscriber : IMessengerEventSubscriber
{
    private readonly IMessageStoreService _messageStoreService;

    public MessageStoreSubscriber(IMessageStoreService messageStoreService)
    {
        _messageStoreService = messageStoreService;
    }

    public async Task ServerStartedAsync(IMessengerServer server)
    {
        await Task.CompletedTask;
    }

    public async Task ServerShutdownAsync(IMessengerServer server)
    {
        await Task.CompletedTask;
    }

    public async Task MessageFromClientAsync(MessageDto messageDto)
    {
        await Task.CompletedTask;
    }

    public async Task MessageFromRouterAsync(MessageDto messageDto)
    {
        await Task.CompletedTask;
    }

    public async Task OnlineAsync(IConnection connection)
    {
        await Task.CompletedTask;
    }

    public async Task OfflineAsync(IConnection connection)
    {
        await Task.CompletedTask;
    }
}