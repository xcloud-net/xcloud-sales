using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using XCloud.Platform.AuthServer.IdentityStore;

namespace XCloud.Platform.AuthServer;

public static class AuthServerConfigurationExtension
{
    public static bool IntegrateIdentityServer(this IConfiguration configuration)
    {
        return configuration["app:identity_server:IntegrateIdentityServer"] == "true";
    }

    public static void ConfigAuthServer(this ServiceConfigurationContext context)
    {
        //identity server
        var config = context.Services.GetConfiguration();
        if (IntegrateIdentityServer(config))
        {
            //ids配置
            context.Services
                .AddIdentityServerComponents()
                .AddOperationStore()
                .AddConfigurationStore();
        }
    }
    
}