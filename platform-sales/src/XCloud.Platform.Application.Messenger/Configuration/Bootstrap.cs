using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using XCloud.Core.Configuration.Builder;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Platform.Application.Messenger.Client;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Handler;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Configuration;

public static class Bootstrap
{
    [Obsolete]
    public static MessengerBuilder AddMessengerServer(this IXCloudBuilder builder)
    {
        var services = builder.Services;

        var handlers = new Assembly[] { typeof(MessengerBuilder).Assembly }.GetAllTypes()
            .Where(x => x.IsNormalPublicClass())
            .Where(x => x.IsAssignableTo_<IMessageHandler>()).ToArray();
        foreach (var h in handlers)
        {
            services.AddSingleton(typeof(IMessageHandler), h);
        }

        return new MessengerBuilder(services);
    }

    public static MessengerBuilder AddDefaultHubProvider(this MessengerBuilder builder)
    {
        builder.Services.AddSingleton(provider => new WsServer(provider, $"{System.Net.Dns.GetHostName()}_"));
        builder.Services.AddSingleton<IWsServer>(provider => provider.GetRequiredService<WsServer>());
        return builder;
    }

    public static IApplicationBuilder UseWebSocketEndpoint(this IApplicationBuilder app, string path)
    {
        return UseWebSocketEndpoint<IWsServer>(app, path);
    }

    public static IApplicationBuilder UseWebSocketEndpoint<T>(this IApplicationBuilder app, string path) where T : IWsServer
    {
        app.Use(async (context, next) =>
        {
            if (Com.RoutePathEqual(context.Request.Path, path))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var me = context.Request.Query["me"];
                    var device = context.Request.Query["device"];

                    var client = new ClientIdentity()
                    {
                        SubjectId = me,
                        DeviceType = device,
                        ConnectionId = context.Connection.Id
                    };

                    var server = context.RequestServices.GetRequiredService<T>();

                    var webSocket = await context.WebSockets.AcceptWebSocketAsync(new WebSocketAcceptContext() { });

                    using var connection = new WsConnection(context.RequestServices, server, webSocket, client);

                    await connection.StartReceiveMessageLoopAsync(CancellationToken.None);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next();
            }
        });
        app.ApplicationServices.GetRequiredService<T>().Start();
        return app;
    }
}