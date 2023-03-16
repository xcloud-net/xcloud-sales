using System.Net.WebSockets;
using FluentAssertions;
using Nito.Disposables;
using XCloud.Core.Extension;
using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Protocol.Websocket;

/// <summary>
/// 需要重写等号运算符
/// </summary>
public class WebsocketConnection : IConnection
{
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly WebSocket _socketChannel;
    public WebsocketConnection(IServiceProvider provider, IMessengerServer server, WebSocket webSocket,
        ClientIdentity clientIdentity)
    {
        provider.Should().NotBeNull(nameof(provider));
        server.Should().NotBeNull(nameof(server));
        webSocket.Should().NotBeNull(nameof(webSocket));
        clientIdentity.Should().NotBeNull(nameof(clientIdentity));

        this.Provider = provider;
        this.ClientIdentity = clientIdentity;
        this.Server = server;
        this._socketChannel = webSocket;

        this._logger = provider.GetRequiredService<ILogger<WebsocketConnection>>();
        this._cancellationTokenSource = new CancellationTokenSource();
    }

    public void RequestAbort() => this._cancellationTokenSource.Cancel();

    public IServiceProvider Provider { get; }
    public IMessengerServer Server { get; }
    public ClientIdentity ClientIdentity { get; }

    public bool IsActive => this._socketChannel.State == WebSocketState.Open;

    public async Task SendMessageToClientAsync(MessageDto data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var bs = this.Server.MessageSerializer.SerializeToBytes(data);
        await this._socketChannel.SendAsync(
            new ArraySegment<byte>(bs),
            WebSocketMessageType.Text,
            true,
            this._cancellationTokenSource.Token);
    }

    private async Task OnMessageFromClient(byte[] bs)
    {
        try
        {
            var data = this.Server.MessageSerializer.DeserializeFromBytes<MessageDto>(bs);
            if (data == null)
                throw new BusinessException("data from clientIdentity is incorrect");

            await this.Server.OnMessageFromClientAsync(data, this);
        }
        catch (Exception e)
        {
            this._logger.AddErrorLog(e.Message, e);
        }
    }

    public async Task StartReceiveMessageLoopAsync(CancellationToken? token = null)
    {
        token ??= CancellationToken.None;

        await using var scope = new AsyncDisposable(async () => await this.Server.OnDisConnectedAsync(this));
        try
        {
            await this.Server.OnConnectedAsync(this);

            await this._socketChannel.StartReceiveLoopAsync(this.OnMessageFromClient,
                receiveDataCancellationToken: token,
                closeConnectionCancellationToken: token);
        }
        catch (Exception e)
        {
            this._logger.AddErrorLog(nameof(StartReceiveMessageLoopAsync), e);
        }
    }

    public void Dispose()
    {
        this.RequestAbort();
        Task.Run(async () =>
        {
            await this._socketChannel.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            this._socketChannel.Dispose();
        }).Wait();
    }
}