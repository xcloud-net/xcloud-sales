using XCloud.Database.EntityFrameworkCore.Repository;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Data.Database;

/// <summary>
/// 基础crud和queryable
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMemberRepository<T> : IPlatformRepository<T>, IEfRepository<T> where T : class, IPlatformEntity
{
    //
}