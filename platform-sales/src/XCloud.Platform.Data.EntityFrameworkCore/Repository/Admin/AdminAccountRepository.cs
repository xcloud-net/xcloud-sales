using XCloud.Core.DependencyInjection;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Data.EntityFrameworkCore.Database;
using XCloud.Platform.Data.Repository.Admin;

namespace XCloud.Platform.Data.EntityFrameworkCore.Repository.Admin;

public class AdminAccountRepository : PlatformRepository<SysAdmin>, IAdminAccountRepository, IAutoRegistered
{
    private readonly IServiceProvider _serviceProvider;
    public AdminAccountRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }
}