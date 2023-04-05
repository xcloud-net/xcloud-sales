using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Hosting;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc;
using XCloud.AspNetMvc.Builder;
using XCloud.AspNetMvc.Middleware;
using XCloud.AspNetMvc.Swagger;
using XCloud.Core.Http.Dynamic;
using XCloud.Gateway.Server.Controller;
using XCloud.Logging.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Core;
using XCloud.Core.Configuration.Builder;

namespace XCloud.Gateway.Server;

[DependsOn(
    typeof(AspNetMvcModule),
    typeof(XCloudGatewayModule)
)]
[SwaggerConfiguration("gateway", "网关", "网关相关接口，不建议调用", mode: SwaggerMode.Gateway)]
public class GatewayServerModule : AbpModule
{
    private void ConfigApiGateway(ServiceConfigurationContext context, IXCloudBuilder builder)
    {
        //builder.Services.AddOcelot(builder.Configuration).AddNacosDiscovery();
        var config = context.Services.GetConfiguration();
        var section = config.GetSection("ReverseProxy");
        if (!section.Exists())
            throw new ConfigException($"ReverseProxy config section is required");
        context.Services.AddReverseProxy().LoadFromConfig(section);
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddXCloudBuilder<GatewayServerModule>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();

        builder.AddDynamicHttpClient<IPostService>();

        this.ConfigApiGateway(context, builder);

        builder.Services.AddWebSockets(option =>
        {
            option.KeepAliveInterval = TimeSpan.FromSeconds(10);
            //option.ReceiveBufferSize = 4;
            option.AllowedOrigins.Add("http://coolaf.com");
            option.AllowedOrigins.Add("http://localhost:4001");
        });
    }

    private void UseCors(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var environment = context.GetEnvironment();
        var configuration = context.GetConfiguration();
        if (environment.IsDevelopment())
        {
            app.UseDevCors();
        }
        else
        {
            app.UseProductionCors(configuration);
        }
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var pipeline = context.CreateMvcPipelineBuilder();

        pipeline.App.UseRouting();
        pipeline.App.UseCorrelationId();
        pipeline.App.UseSerilogAspNetCoreEnricher();

        pipeline.App.UseStaticFiles();

        pipeline.TryEnableSwagger();
        //审计日志
        pipeline.App.UseAuditing();

        if (pipeline.Environment.IsDevelopment())
        {
            pipeline.App.UseDeveloperExceptionPage();
            pipeline.UseDevelopmentInformation();
        }

        this.UseCors(context);
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var pipeline = context.CreateMvcPipelineBuilder();

        pipeline.App.UseWebSockets();
        pipeline.App.UseEndpoints(e =>
        {
            e.MapDefaultControllerRoute();
            e.MapReverseProxy();
        });
        //注册网关
        //Task.Run(() => pipeline.App.UseOcelot()).Wait();
    }
}