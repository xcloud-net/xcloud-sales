using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nito.Disposables;
using XCloud.Core.Extension;

namespace XCloud.Platform.Application.Common.Service.Messenger.Connection;

/// <summary>
/// 需要重写等号运算符
/// </summary>
public class WsConnection : IDisposable
{
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    public WsConnection(IServiceProvider provider, IWsServer server, WebSocket webSocket, WsClient client)
    {
        provider.Should().NotBeNull(nameof(provider));
        server.Should().NotBeNull(nameof(server));
        webSocket.Should().NotBeNull(nameof(webSocket));
        client.Should().NotBeNull(nameof(client));

        this.Provider = provider;
        this.Client = client;
        this.Server = server;
        this.SocketChannel = webSocket;

        this._logger = provider.GetRequiredService<ILogger<WsConnection>>();
        this._cancellationTokenSource = new CancellationTokenSource();
    }

    public void ReuqestAbort() => this._cancellationTokenSource.Cancel();

    public IServiceProvider Provider { get; }
    public IWsServer Server { get; }
    public WebSocket SocketChannel { get; }
    public WsClient Client { get; }

    public async Task SendMessage(MessageWrapper data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(SendMessage));

        var bs = this.Server.MessageSerializer.SerializeToBytes(data);
        await this.SocketChannel.SendAsync(
            new ArraySegment<byte>(bs),
            WebSocketMessageType.Text,
            true,
            this._cancellationTokenSource.Token);
    }

    async Task OnMessageFromClient(byte[] bs)
    {
        try
        {
            var data = this.Server.MessageSerializer.DeserializeFromBytes<MessageWrapper>(bs);
            if (data == null)
                throw new BusinessException("data from client is incorrect");

            await this.Server.OnMessageFromClient(data, this);
        }
        catch (Exception e)
        {
            this._logger.AddErrorLog(e.Message, e);
        }
    }

    public async Task CloseAsync(CancellationToken? token = null)
    {
        token ??= CancellationToken.None;
        await this.SocketChannel.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, token.Value);
    }

    public async Task StartReceiveMessageLoopAsync(CancellationToken? token = null)
    {
        token ??= CancellationToken.None;

        await using var scope = new AsyncDisposable(async () => await this.Server.OnClientLeave(this));
        try
        {
            await this.Server.OnClientJoin(this);

            await this.SocketChannel.StartReceiveLoopAsync((bs) => this.OnMessageFromClient(bs));
        }
        catch (Exception e)
        {
            this._logger.AddErrorLog(nameof(StartReceiveMessageLoopAsync), e);
        }
    }

    public void Dispose()
    {
        this.SocketChannel.Dispose();
    }
}