global using System;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Volo.Abp;
global using System.Threading.Tasks;
global using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Core.Configuration.Builder;
using XCloud.Job;
using XCloud.Platform.Application.Messenger.Configuration;
using XCloud.Platform.Application.Messenger.Protocol.Websocket;
using XCloud.Platform.Application.Messenger.Router;
using XCloud.Platform.Application.Messenger.Settings;
using XCloud.Platform.Auth;
using XCloud.Platform.Shared.Constants;
using XCloud.Redis;

namespace XCloud.Platform.Application.Messenger;

[DependsOn(
    typeof(JobModule),
    typeof(RedisModule),
    typeof(PlatformAuthModule))]
public class PlatformMessengerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();

        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformMessengerModule>(validate: false));
        this.Configure<PlatformMessengerOption>(option => { option.Enabled = false; });

        context.Services.AddWebSockets(option =>
        {
            option.KeepAliveInterval = TimeSpan.FromSeconds(60);
            option.AllowedOrigins.Add("*");
        });

        builder.AddMessengerServer()
            .AddDefaultHubProvider();
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.RemoveAll<IMessageRouter>();
        context.Services.AddSingleton<IMessageRouter, InMemoryMessageRouter>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        //web socket
        app.UseWebSockets();
        app.UseWebSocketEndpoint($"/api/{PlatformSharedConstants.ServiceName}-ws/ws");
    }
}