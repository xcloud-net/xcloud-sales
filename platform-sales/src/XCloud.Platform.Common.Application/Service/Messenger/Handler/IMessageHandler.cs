using System.Threading.Tasks;
using XCloud.Platform.Common.Application.Service.Messenger.Connection;

namespace XCloud.Platform.Common.Application.Service.Messenger.Handler;

public abstract class MessageContextBase
{
    public IWsServer WsServer { get; set; }
    public IServiceProvider HandleServiceProvider { get; set; }
    public MessageWrapper Message { get; set; }
}

public class ClientMessageContext : MessageContextBase
{
    public ClientMessageContext(IWsServer wsServer)
    {
        this.WsServer = wsServer;
    }

    public WsConnection Connection { get; set; }
}

public class TransportMessageContext : MessageContextBase
{
    public TransportMessageContext(IWsServer wsServer)
    {
        this.WsServer = wsServer;
    }
}

public interface IMessageHandler
{
    string MessageType { get; }
    
    int Sort { get; }

    Task HandleMessageFromClientAsync(ClientMessageContext context);
    Task HandleMessageFromTransportAsync(TransportMessageContext context);
}