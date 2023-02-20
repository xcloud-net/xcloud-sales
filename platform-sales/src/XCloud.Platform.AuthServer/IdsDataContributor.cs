using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Application.WorkContext;
using XCloud.Platform.AuthServer.IdentityStore.IdsDbContext;

namespace XCloud.Platform.AuthServer;

public class IdsDataContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IWorkContext _context;
    public IdsDataContributor(IWorkContext<IdsDataContributor> context)
    {
        this._context = context;
    }

    public async Task SeedAsync(DataSeedContext dataSeedContext)
    {
        using var s = this._context.ServiceProvider.CreateScope();

        var logger = s.ServiceProvider.GetRequiredService<ILogger<IdsDataContributor>>();

        //尝试创建client，resource等配置数据库
        var configDb = s.ServiceProvider.GetService<IdentityConfigurationDbContext>();
        if (configDb != null)
        {
            using (configDb)
            {
                await configDb.Database.EnsureCreatedAsync();
                logger.LogInformation("创建ids配置库");
            }
        }

        //尝试创建token的授权数据库
        var grantsDb = s.ServiceProvider.GetService<IdentityOperationDbContext>();
        if (grantsDb != null)
        {
            using (grantsDb)
            {
                await grantsDb.Database.EnsureCreatedAsync();
                logger.LogInformation("创建ids授权库");
            }
        }
    }
}