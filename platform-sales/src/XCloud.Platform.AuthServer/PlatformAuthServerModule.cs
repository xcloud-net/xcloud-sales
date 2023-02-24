using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.Auth;
using XCloud.Platform.AuthServer.IdentityServer;
using XCloud.Platform.AuthServer.IdentityServer.PersistentStore;

namespace XCloud.Platform.AuthServer;

[DependsOn(
    typeof(MySqlDatabaseModule),
    typeof(PlatformAuthModule)
)]
public class PlatformAuthServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AuthServerDatabaseOption>(option => { option.AutoCreateDatabase = false; });

        context.ConfigAuthServer();

        Configure<AbpAutoMapperOptions>(option => { option.AddMaps<PlatformAuthServerModule>(validate: false); });
    }
}