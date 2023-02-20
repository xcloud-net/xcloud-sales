using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Framework.DataSeeder;

public class MemberDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly EnsureSuperUserCreatedService _ensureSuperUserCreatedService;

    public MemberDataSeedContributor(EnsureSuperUserCreatedService ensureSuperUserCreatedService)
    {
        _ensureSuperUserCreatedService = ensureSuperUserCreatedService;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await this._ensureSuperUserCreatedService.EnsureSuperUserCreatedAsync();
    }
}