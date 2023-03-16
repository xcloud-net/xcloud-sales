using System.Net;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Authorization;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Auth.Application.User;

namespace XCloud.Platform.Application.Messenger.Protocol.Websocket;

public class EndpointMiddleware : IMiddleware
{
    private readonly string _path;

    public EndpointMiddleware(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        
        _path = path;
    }

    private async Task<ClientIdentity> GetRequiredClientIdentityAsync(HttpContext context)
    {
        var device = context.Request.Query["device"];
        if (string.IsNullOrWhiteSpace(device))
            throw new ArgumentNullException(nameof(device));
        
        using var s = context.RequestServices.CreateScope();
        
        var handler = s.ServiceProvider.GetRequiredService<IdentityServerAuthenticationHandler>();

        var result = await handler.AuthenticateAsync();
        
        var userAuthService = s.ServiceProvider.GetRequiredService<IUserAuthService>();

        var response = await userAuthService.GetAuthUserAsync();

        response.ThrowIfErrorOccured();

        var client = new ClientIdentity()
        {
            SubjectId = response.Data.Id,
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

        using IConnection connection = new WebsocketConnection(context.RequestServices, server, webSocket, client);

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