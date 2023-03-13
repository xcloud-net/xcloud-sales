using FluentAssertions;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Handler;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Registry;
using XCloud.Platform.Application.Messenger.Router;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Server;

public interface IWsServer : IDisposable
{
    ClientManager ClientManager { get; }

    /// <summary>
    /// 用于路由
    /// </summary>
    string ServerInstanceId { get; }

    IJsonDataSerializer MessageSerializer { get; }
    IRegistrationProvider RegistrationProvider { get; }
    IPersistenceProvider PersistenceProvider { get; }
    ITransportProvider TransportProvider { get; }
    IMessageHandler[] MessageHandlers { get; }

    Task OnClientJoin(WsConnection wsConnection);
    Task OnClientLeave(WsConnection wsConnection);


    Task OnMessageFromClient(MessageWrapper message, WsConnection wsConnection);
    Task OnMessageFromTransport(MessageWrapper message);

    IMessageHandler GetHandlerOrNull(string type);
    Task Cleanup();
    Task Start();
}

public class WsServer : IWsServer
{
    private readonly ILogger logger;

    public ClientManager ClientManager { get; }

    /// <summary>
    /// 用于路由
    /// </summary>
    public string ServerInstanceId { get; }

    public IServiceProvider ServiceProvider { get; }
    public IJsonDataSerializer MessageSerializer { get; }
    public IRegistrationProvider RegistrationProvider { get; }
    public IPersistenceProvider PersistenceProvider { get; }
    public ITransportProvider TransportProvider { get; }
    public IMessageHandler[] MessageHandlers { get; }

    public WsServer(IServiceProvider provider, string serverInstanceId)
    {
        serverInstanceId.Should().NotBeNullOrEmpty();

        this.logger = provider.GetRequiredService<ILogger<WsServer>>();

        this.ClientManager = new ClientManager();
        this.ServerInstanceId = serverInstanceId;

        this.ServiceProvider = provider;
        this.MessageSerializer = provider.GetRequiredService<IJsonDataSerializer>();
        this.RegistrationProvider = provider.GetRequiredService<IRegistrationProvider>();
        this.PersistenceProvider = provider.GetRequiredService<IPersistenceProvider>();
        this.TransportProvider = provider.GetRequiredService<ITransportProvider>();
        this.MessageHandlers = provider.GetServices<IMessageHandler>().ToArray();
    }

    public async Task OnClientJoin(WsConnection wsConnection)
    {
        this.ClientManager.AddConnection(wsConnection);

        //ping will trigger registration work
        var msg = new MessageWrapper() { MessageType = MessageTypeConst.Ping };
        await this.OnMessageFromClient(msg, wsConnection);

        this.logger.LogInformation($"{wsConnection.Client.ConnectionId} joined");
    }

    public async Task OnClientLeave(WsConnection wsConnection)
    {
        this.ClientManager.RemoveConnection(wsConnection);
        await this.RegistrationProvider.RemoveRegisterInfoAsync(wsConnection.Client.SubjectId, wsConnection.Client.DeviceType);

        this.logger.LogInformation($"{wsConnection.Client.ConnectionId} leaved");
    }

    public async Task OnMessageFromClient(MessageWrapper message, WsConnection wsConnection)
    {
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
            this.logger.LogWarning("no handler for this message type");
            await wsConnection.SendMessage(new MessageWrapper() { MessageType = "no handler for this message type" });
        }
    }

    public async Task OnMessageFromTransport(MessageWrapper message)
    {
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
            this.logger.LogWarning(message: $"no handler for message:{this.MessageSerializer.SerializeToString(message)}");
        }
    }

    public async Task Start()
    {
        //路由到这台服务器的消息
        var queueKey = this.ServerInstanceId;
        await this.TransportProvider.SubscribeMessageEndpoint(queueKey, this.OnMessageFromTransport);
        //路由到所有服务器的消息
        await this.TransportProvider.SubscribeBroadcastMessageEndpoint(this.OnMessageFromTransport);
    }

    public void Dispose()
    {
        Task.Run(this.Cleanup).Wait();
        this.ClientManager.Dispose();
        this.TransportProvider.Dispose();
    }

    public IMessageHandler GetHandlerOrNull(string type)
    {
        var handler = this.MessageHandlers
            .OrderByDescending(x => x.Sort)
            .FirstOrDefault(x => x.MessageType == type);
        return handler;
    }

    public async Task Cleanup()
    {
        await Task.CompletedTask;
    }
}