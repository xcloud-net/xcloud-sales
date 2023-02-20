using Volo.Abp.Domain.Entities;
using XCloud.Core.Helper;

namespace XCloud.Core.Extension;

/// <summary>
/// 对Linq的扩展
/// </summary>
public static class QueryableExtensions
{
    public static IQueryable<T> WhereIdIn<T, TKey>(this IQueryable<T> query, IEnumerable<TKey> ids)
        where T : IEntity<TKey>
    {
        return query.Where(x => ids.Contains(x.Id));
    }

    /// <summary>
    /// 分页
    /// </summary>
    public static IQueryable<T> QueryPage<T>(this IOrderedQueryable<T> query, int page, int pageSize)
    {
        var skip = Com.GetPagedSkip(page, pageSize);

        IQueryable<T> q = query;

        if (skip > 0)
            q = q.Skip(skip);

        var res = q.Take(pageSize);
        return res;
    }
}