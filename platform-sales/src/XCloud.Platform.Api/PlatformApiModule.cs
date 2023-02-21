global using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc;
using XCloud.AspNetMvc.Builder;
using XCloud.AspNetMvc.Configuration;
using XCloud.AspNetMvc.Swagger;
using XCloud.Core.Builder;
using XCloud.Platform.AuthServer;
using XCloud.Platform.Common.Application.Service.Messenger;
using XCloud.Platform.Framework;
using XCloud.Platform.Member.Application;

namespace XCloud.Platform.Api;

[DependsOn(
    typeof(PlatformFrameworkModule),
    typeof(PlatformAuthServerModule)
)]
[SwaggerConfiguration(ServiceName, ServiceName)]
public class PlatformApiModule : AbpModule
{
    public PlatformApiModule()
    {
        //
    }

    private const string ServiceName = "platform";
    
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddXCloudBuilder<PlatformApiModule>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //PlatformMemberModule
        context.Services.GetMvcBuilder().AddApplicationPartIfNotExists(typeof(PlatformMemberModule).Assembly);
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var pipeline = context.CreateMvcPipelineBuilder();

        //multiple language
        pipeline.App.UseAbpRequestLocalization();
        
        if (pipeline.Configuration.IntegrateIdentityServer())
        {
            pipeline.SetIdentityPublicOrigin();
            //UseIdentityServer includes a call to UseAuthentication,
            //so it’s not necessary to have both.
            pipeline.App.UseIdentityServer();
            pipeline.App.UseAuthorization();
        }
        else
        {
            pipeline.App.UseAuthentication().UseAuthorization();
        }
        
        //审计日志
        pipeline.App.UseAuditing();
        
        //web socket
        pipeline.App.UseWebSockets();
        pipeline.App.UseWebSocketEndpoint($"/api/{ServiceName}-ws/ws");
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var pipeline = context.CreateMvcPipelineBuilder();
        
        pipeline.App.UseEndpoints(e => { e.MapDefaultControllerRoute(); });
    }
}