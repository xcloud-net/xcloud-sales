using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Handler;

public abstract class MessageContextBase
{
    public IMessengerServer MessengerServer { get; set; }
    public IServiceProvider HandleServiceProvider { get; set; }
    public MessageDto Message { get; set; }
}

public class ClientMessageContext : MessageContextBase
{
    public ClientMessageContext(IMessengerServer messengerServer)
    {
        this.MessengerServer = messengerServer;
    }

    public IConnection Connection { get; set; }
}

public class TransportMessageContext : MessageContextBase
{
    public TransportMessageContext(IMessengerServer messengerServer)
    {
        this.MessengerServer = messengerServer;
    }
}

public interface IMessageHandler
{
    string MessageType { get; }
    
    int Sort { get; }

    Task HandleMessageFromClientAsync(ClientMessageContext context);
    Task HandleMessageFromTransportAsync(TransportMessageContext context);
}