using System.Collections.Generic;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Protocol.Websocket;

public static class WebsocketExtension
{
    internal static IApplicationBuilder UseWebSocketEndpoint(this IApplicationBuilder app, string path)
    {
        app.UseMiddleware<EndpointMiddleware>(args: new object[] { path });
        app.ApplicationServices.GetRequiredService<IMessengerServer>().StartAsync();
        return app;
    }

    internal static async Task StartReceiveLoopAsync(this WebSocket ws,
        Func<byte[], Task> onMessage,
        CancellationToken? receiveDataCancellationToken = null,
        CancellationToken? closeConnectionCancellationToken = null,
        int? bufferSize = null)
    {
        receiveDataCancellationToken ??= CancellationToken.None;
        closeConnectionCancellationToken ??= CancellationToken.None;
        bufferSize ??= 1024 * 4;

        var buffer = new byte[bufferSize.Value];

        var closeStatus = WebSocketCloseStatus.NormalClosure;
        try
        {
            var data = new List<byte>();

            while (ws.State == WebSocketState.Open)
            {
                if (receiveDataCancellationToken.Value.IsCancellationRequested)
                    break;

                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), receiveDataCancellationToken.Value);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        if (result.Count <= 0)
                            await Task.Delay(TimeSpan.FromMilliseconds(100));

                        data.AddRange(buffer.Take(result.Count));

                        if (data.Count > 1024 * 1024 * 0.5)
                            throw new NotSupportedException("message is too big,server is closing this connection");

                        if (result.EndOfMessage)
                        {
                            var bs = data.ToArray();
                            await onMessage.Invoke(bs);
                            //reset
                            data = new List<byte>() { };
                        }

                        continue;
                    case WebSocketMessageType.Binary:
                        throw new NotSupportedException();
                    case WebSocketMessageType.Close:
                        if (result.CloseStatus != null)
                            closeStatus = result.CloseStatus.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        finally
        {
            var closeDescription = Enum.GetName(typeof(WebSocketCloseStatus), closeStatus);
            await ws.CloseAsync(closeStatus, closeDescription, closeConnectionCancellationToken.Value);
        }
    }
}