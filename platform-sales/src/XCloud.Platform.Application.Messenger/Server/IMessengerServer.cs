using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Handler;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Registry;
using XCloud.Platform.Application.Messenger.Router;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Server;

public interface IMessengerServer : IDisposable
{
    ConnectionManager ConnectionManager { get; }

    /// <summary>
    /// 用于路由
    /// </summary>
    string ServerInstanceId { get; }

    IJsonDataSerializer MessageSerializer { get; }
    IRegistrationProvider RegistrationProvider { get; }
    IMessageStoreService MessageStoreService { get; }
    IMessageRouter MessageRouter { get; }
    IMessageHandler[] MessageHandlers { get; }

    Task OnClientJoinAsync(IConnection wsConnection);
    Task OnClientLeaveAsync(IConnection wsConnection);


    Task OnMessageFromClientAsync(MessageWrapper message, IConnection wsConnection);
    Task OnMessageFromRouterAsync(MessageWrapper message);

    IMessageHandler GetHandlerOrNull(string type);
    
    Task CleanupAsync();
    Task StartAsync();
}