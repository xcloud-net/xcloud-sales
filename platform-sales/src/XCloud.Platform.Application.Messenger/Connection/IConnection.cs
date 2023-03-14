using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Connection;

public interface IConnection : IDisposable
{
    IServiceProvider Provider { get; }
    
    IMessengerServer Server { get; }
    
    ClientIdentity ClientIdentity { get; }
    
    void RequestAbort();
    
    Task SendMessageToClientAsync(MessageWrapper data);
    
    Task CloseAsync(CancellationToken? token = null);
    
    Task StartReceiveMessageLoopAsync(CancellationToken? token = null);
}