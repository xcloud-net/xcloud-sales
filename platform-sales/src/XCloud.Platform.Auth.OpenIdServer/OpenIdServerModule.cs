using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.Platform.Auth.OpenIdServer.Configuration;
using XCloud.Platform.Framework;

namespace XCloud.Platform.Auth.OpenIdServer;

[DependsOn(
    typeof(PlatformFrameworkModule)
)]
public class OpenIdServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.AddOpenIdDictDbContext();
        context.AddOpenIdDictServer();
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.ApplicationServices.TryCreateOpenIdDictDatabase();
    }
}