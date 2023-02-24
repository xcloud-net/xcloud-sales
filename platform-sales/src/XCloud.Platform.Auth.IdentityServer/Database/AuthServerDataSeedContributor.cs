using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Application.WorkContext;

namespace XCloud.Platform.Auth.IdentityServer.Database;

public class AuthServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IWorkContext _context;

    public AuthServerDataSeedContributor(IWorkContext<AuthServerDataSeedContributor> context)
    {
        this._context = context;
    }


    public async Task SeedAsync(DataSeedContext dataSeedContext)
    {
        await Task.CompletedTask;

        using var s = this._context.ServiceProvider.CreateScope();

        //s.ServiceProvider.AutoCreateIdentityServerDatabase();
    }
}