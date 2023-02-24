using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Application.WorkContext;
using XCloud.Platform.AuthServer.IdentityServer.PersistentStore.EntityFrameworkCore;

namespace XCloud.Platform.AuthServer.IdentityServer.PersistentStore;

public class AuthServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IWorkContext _context;

    public AuthServerDataSeedContributor(IWorkContext<AuthServerDataSeedContributor> context)
    {
        this._context = context;
    }

    private async Task AutoCreateIdentityServerDatabase(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<AuthServerDataSeedContributor>>();

        if (!serviceProvider.AutoCreateAuthServerDatabase())
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
                await configDb.Database.EnsureCreatedAsync();
                logger.LogInformation("创建ids配置库");
            }
        }

        //尝试创建token的授权数据库
        var grantsDb = serviceProvider.GetService<IdentityOperationDbContext>();
        if (grantsDb != null)
        {
            using (grantsDb)
            {
                await grantsDb.Database.EnsureCreatedAsync();
                logger.LogInformation("创建ids授权库");
            }
        }
    }

    public async Task SeedAsync(DataSeedContext dataSeedContext)
    {
        using var s = this._context.ServiceProvider.CreateScope();

        await this.AutoCreateIdentityServerDatabase(s.ServiceProvider);
    }
}