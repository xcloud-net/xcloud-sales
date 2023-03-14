using FluentAssertions;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Constants;
using XCloud.Platform.Application.Messenger.Event;
using XCloud.Platform.Application.Messenger.Handler;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Registry;
using XCloud.Platform.Application.Messenger.Router;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Server;

public class MessengerServer : IMessengerServer
{
    private readonly ILogger _logger;

    public ConnectionManager ConnectionManager { get; }

    /// <summary>
    /// 用于路由
    /// </summary>
    public string ServerInstanceId { get; }

    public IServiceProvider ServiceProvider { get; }
    public IMessengerEventManager MessengerEventManager { get; }
    public IJsonDataSerializer MessageSerializer { get; }
    public IRegistrationProvider RegistrationProvider { get; }
    public IMessageRouter MessageRouter { get; }
    public IMessageHandler[] MessageHandlers { get; }

    public MessengerServer(IServiceProvider provider, string serverInstanceId)
    {
        serverInstanceId.Should().NotBeNullOrEmpty();

        this._logger = provider.GetRequiredService<ILogger<MessengerServer>>();

        this.ConnectionManager = new ConnectionManager();
        this.ServerInstanceId = serverInstanceId;

        this.ServiceProvider = provider;
        this.MessengerEventManager = provider.GetRequiredService<IMessengerEventManager>();
        this.MessageSerializer = provider.GetRequiredService<IJsonDataSerializer>();
        this.RegistrationProvider = provider.GetRequiredService<IRegistrationProvider>();
        this.MessageRouter = provider.GetRequiredService<IMessageRouter>();
        this.MessageHandlers = provider.GetServices<IMessageHandler>().ToArray();
    }

    public async Task OnClientJoinAsync(IConnection wsConnection)
    {
        this.ConnectionManager.AddConnection(wsConnection);

        //ping will trigger registration work
        var msg = new MessageWrapper() { MessageType = MessageTypeConst.Ping };
        
        await this.OnMessageFromClientAsync(msg, wsConnection);

        await this.MessengerEventManager.OnlineAsync(wsConnection);
        
        this._logger.LogInformation($"{wsConnection.ClientIdentity.ConnectionId} joined");
    }

    public async Task OnClientLeaveAsync(IConnection wsConnection)
    {
        this.ConnectionManager.RemoveConnection(wsConnection);
        
        await this.RegistrationProvider.RemoveRegisterInfoAsync(wsConnection.ClientIdentity.SubjectId, wsConnection.ClientIdentity.DeviceType);

        await this.MessengerEventManager.OfflineAsync(wsConnection);
        
        this._logger.LogInformation($"{wsConnection.ClientIdentity.ConnectionId} leaved");
    }

    public async Task OnMessageFromClientAsync(MessageWrapper message, IConnection wsConnection)
    {
        await this.MessengerEventManager.MessageFromClientAsync(message);
        
        var handler = this.GetHandlerOrNull(message.MessageType);
        if (handler != null)
        {
            using var s = this.ServiceProvider.CreateScope();
            var context = new ClientMessageContext(this)
            {
                HandleServiceProvider = s.ServiceProvider,
                Connection = wsConnection,
                Message = message
            };
            await handler.HandleMessageFromClientAsync(context);
        }
        else
        {
            this._logger.LogWarning("no handler for this message type");
            await wsConnection.SendMessageToClientAsync(new MessageWrapper() { MessageType = "no handler for this message type" });
        }
    }

    public async Task OnMessageFromRouterAsync(MessageWrapper message)
    {
        await this.MessengerEventManager.MessageFromRouterAsync(message);
        
        var handler = this.GetHandlerOrNull(message.MessageType);
        if (handler != null)
        {
            using var s = this.ServiceProvider.CreateScope();
            var context = new TransportMessageContext(this)
            {
                HandleServiceProvider = s.ServiceProvider,
                Message = message
            };
            await handler.HandleMessageFromTransportAsync(context);
        }
        else
        {
            this._logger.LogWarning(message: $"no handler for message:{this.MessageSerializer.SerializeToString(message)}");
        }
    }

    public async Task StartAsync()
    {
        //路由到这台服务器的消息
        var queueKey = this.ServerInstanceId;
        await this.MessageRouter.SubscribeMessageEndpoint(queueKey, this.OnMessageFromRouterAsync);
        //路由到所有服务器的消息
        await this.MessageRouter.SubscribeBroadcastMessageEndpoint(this.OnMessageFromRouterAsync);

        await this.MessengerEventManager.ServerStartedAsync(this);
    }

    public void Dispose()
    {
        Task.Run(() => this.MessengerEventManager.ServerShutdownAsync(this)).Wait();
        Task.Run(this.CleanupAsync).Wait();
        this.ConnectionManager.Dispose();
        this.MessageRouter.Dispose();
    }

    public IMessageHandler GetHandlerOrNull(string type)
    {
        var handler = this.MessageHandlers
            .OrderByDescending(x => x.Sort)
            .FirstOrDefault(x => x.MessageType == type);
        return handler;
    }

    public async Task CleanupAsync()
    {
        await Task.CompletedTask;
    }
}