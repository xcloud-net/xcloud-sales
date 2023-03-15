using System.Net;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Authorization;
using XCloud.Core.Helper;
using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Auth.Application.User;

namespace XCloud.Platform.Application.Messenger.Protocol.Websocket;

public class EndpointMiddleware : IMiddleware
{
    private readonly string _path;

    public EndpointMiddleware(string path)
    {
        _path = path;
    }

    private async Task<ClientIdentity> GetRequiredClientIdentityAsync(HttpContext context)
    {
        using var s = context.RequestServices.CreateScope();
        var userAuthService = s.ServiceProvider.GetRequiredService<IUserAuthService>();

        var response = await userAuthService.GetAuthUserAsync();

        var me = context.Request.Query["me"];
        var device = context.Request.Query["device"];

        var client = new ClientIdentity()
        {
            SubjectId = me,
            DeviceType = device,
            ConnectionId = context.Connection.Id
        };
        throw new NotImplementedException();
    }

    private async Task ReceiveConnectionAsync(HttpContext context)
    {
        var client = await this.GetRequiredClientIdentityAsync(context);

        var server = context.RequestServices.GetRequiredService<IMessengerServer>();

        var webSocket = await context.WebSockets.AcceptWebSocketAsync(new WebSocketAcceptContext() { });

        using var connection = new WebsocketConnection(context.RequestServices, server, webSocket, client);

        await connection.StartReceiveMessageLoopAsync(CancellationToken.None);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (Com.RoutePathEqual(context.Request.Path, this._path))
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    await this.ReceiveConnectionAsync(context);
                }
                catch (AbpAuthorizationException e)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        else
        {
            await next.Invoke(context);
        }
    }
}