using System;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;
using XCloud.Core.Configuration.Builder;
using XCloud.Platform.Application.Messenger.Configuration;
using XCloud.Platform.Application.Messenger.Router;

namespace XCloud.Platform.Framework.Configuration;

public static class MessengerExtension
{
    public static void ConfigMessenger(this ServiceConfigurationContext context, IXCloudBuilder builder)
    {
        builder.AddMessengerServer()
            .AddDefaultHubProvider();
    }

    public static void ConfigMessengerTransport(this ServiceConfigurationContext context)
    {
        context.Services.RemoveAll<IMessageRouter>();
        context.Services.AddSingleton<IMessageRouter, InMemoryMessageRouter>();
    }

    public static void ConfigWebsocket(this ServiceConfigurationContext context)
    {
        context.Services.AddWebSockets(option =>
        {
            option.KeepAliveInterval = TimeSpan.FromSeconds(60);
            //option.ReceiveBufferSize = 4;
            option.AllowedOrigins.Add("http://coolaf.com");
            option.AllowedOrigins.Add("http://localhost:4001");
        });
    }
    
}