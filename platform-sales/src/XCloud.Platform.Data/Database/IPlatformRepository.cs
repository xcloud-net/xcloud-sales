using XCloud.Database.EntityFrameworkCore.Repository;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Data.Database;

public interface IPlatformRepository<T> : IEfRepository<T> where T : class, IPlatformEntity
{
    //
}