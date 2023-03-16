using FluentAssertions;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Constants;
using XCloud.Platform.Application.Messenger.Event;
using XCloud.Platform.Application.Messenger.Handler;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Router;
using XCloud.Platform.Application.Messenger.Service;
using XCloud.Platform.Application.Messenger.Tasks;

namespace XCloud.Platform.Application.Messenger.Server;

public interface IMessengerServer : IDisposable
{
    ConnectionManager ConnectionManager { get; }

    /// <summary>
    /// 用于路由
    /// </summary>
    string ServerInstanceId { get; }

    IJsonDataSerializer MessageSerializer { get; }
    IUserRegistrationService UserRegistrationService { get; }
    IMessageRouter MessageRouter { get; }

    Task OnConnectedAsync(IConnection wsConnection);
    Task OnDisConnectedAsync(IConnection wsConnection);
    
    Task OnMessageFromClientAsync(MessageDto message, IConnection wsConnection);
    Task OnMessageFromRouterAsync(MessageDto message);

    Task StartAsync();
}

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
    public IUserRegistrationService UserRegistrationService { get; }
    public IMessageRouter MessageRouter { get; }
    public IMessageHandler[] MessageHandlers { get; }
    public IMessengerTaskManager MessengerTaskManager { get; }

    public MessengerServer(IServiceProvider provider, string serverInstanceId)
    {
        serverInstanceId.Should().NotBeNullOrEmpty();

        this._logger = provider.GetRequiredService<ILogger<MessengerServer>>();

        this.ConnectionManager = new ConnectionManager();
        this.ServerInstanceId = serverInstanceId;

        this.ServiceProvider = provider;
        this.MessengerEventManager = provider.GetRequiredService<IMessengerEventManager>();
        this.MessengerTaskManager = provider.GetRequiredService<IMessengerTaskManager>();
        this.MessageSerializer = provider.GetRequiredService<IJsonDataSerializer>();
        this.UserRegistrationService = provider.GetRequiredService<IUserRegistrationService>();
        this.MessageRouter = provider.GetRequiredService<IMessageRouter>();
        this.MessageHandlers = provider.GetServices<IMessageHandler>().ToArray();
    }

    public async Task OnConnectedAsync(IConnection wsConnection)
    {
        this.ConnectionManager.AddConnection(wsConnection);

        //ping will trigger registration work
        var msg = new MessageDto() { MessageType = MessageTypeConst.Ping };

        await this.OnMessageFromClientAsync(msg, wsConnection);

        await this.MessengerEventManager.OnlineAsync(wsConnection);

        this._logger.LogInformation($"{wsConnection.ClientIdentity.ConnectionId} joined");
    }

    public async Task OnDisConnectedAsync(IConnection wsConnection)
    {
        this.ConnectionManager.RemoveConnection(wsConnection);

        await this.UserRegistrationService.RemoveRegisterInfoAsync(wsConnection.ClientIdentity.SubjectId,
            wsConnection.ClientIdentity.DeviceType);

        await this.MessengerEventManager.OfflineAsync(wsConnection);

        this._logger.LogInformation($"{wsConnection.ClientIdentity.ConnectionId} leaved");
    }

    public async Task OnMessageFromClientAsync(MessageDto message, IConnection wsConnection)
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
            await wsConnection.SendMessageToClientAsync(new MessageDto()
                { MessageType = "no handler for this message type" });
        }
    }

    public async Task OnMessageFromRouterAsync(MessageDto message)
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
            this._logger.LogWarning(
                message: $"no handler for message:{this.MessageSerializer.SerializeToString(message)}");
        }
    }

    public async Task StartAsync()
    {
        //路由到这台服务器的消息
        var queueKey = this.ServerInstanceId;
        await this.MessageRouter.SubscribeMessageEndpoint(queueKey, this.OnMessageFromRouterAsync);

        this.MessengerTaskManager.StartTasks();

        await this.MessengerEventManager.ServerStartedAsync(this);
    }

    public void Dispose()
    {
        Task.Run(() => this.MessengerEventManager.ServerShutdownAsync(this)).Wait();
        
        this.MessengerTaskManager.Dispose();
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
}