using XCloud.Database.EntityFrameworkCore.Repository;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Data.EntityFrameworkCore.Database;

/// <summary>
/// 会员中心仓储实现
/// </summary>
public class PlatformRepository<T> : EfRepositoryBase<PlatformDbContext, T>, IMemberRepository<T> where T : class, IPlatformEntity
{
    public PlatformRepository(IServiceProvider provider) : base(provider) { }
}