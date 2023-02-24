using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.Auth.IdentityServer.Configuration;

namespace XCloud.Platform.Auth.IdentityServer;

[DependsOn(
    typeof(MySqlDatabaseModule),
    typeof(PlatformAuthModule)
)]
public class PlatformIdentityServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigIdentityServer();

        Configure<AbpAutoMapperOptions>(option => { option.AddMaps<PlatformIdentityServerModule>(validate: false); });
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.ApplicationServices.TryCreateIdentityServerDatabase();
    }
}