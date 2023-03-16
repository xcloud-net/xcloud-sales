using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Connection;

public interface IConnection : IDisposable
{
    IServiceProvider Provider { get; }

    bool IsActive { get; }

    IMessengerServer Server { get; }
    
    ClientIdentity ClientIdentity { get; }

    Task SendMessageToClientAsync(MessageDto data);

    Task StartReceiveMessageLoopAsync(CancellationToken? token = null);
}