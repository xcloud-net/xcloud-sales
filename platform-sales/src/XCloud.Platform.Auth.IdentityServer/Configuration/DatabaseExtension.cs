using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XCloud.Platform.Auth.IdentityServer.Database;
using XCloud.Platform.Auth.IdentityServer.Database.EntityFrameworkCore;
using XCloud.Platform.Core;

namespace XCloud.Platform.Auth.IdentityServer.Configuration;

public static class DatabaseExtension
{
    public static void TryCreateIdentityServerDatabase(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<AuthServerDataSeedContributor>>();

        if (!serviceProvider.AutoCreatePlatformDatabase())
        {
            logger.LogInformation("skip create identity server database");
            return;
        }

        //尝试创建client，resource等配置数据库
        var configDb = serviceProvider.GetService<IdentityConfigurationDbContext>();
        if (configDb != null)
        {
            using (configDb)
            { 
                configDb.Database.EnsureCreated();
                logger.LogInformation("创建ids配置库");
            }
        }

        //尝试创建token的授权数据库
        var grantsDb = serviceProvider.GetService<IdentityOperationDbContext>();
        if (grantsDb != null)
        {
            using (grantsDb)
            { 
                grantsDb.Database.EnsureCreated();
                logger.LogInformation("创建ids授权库");
            }
        }
    }
}