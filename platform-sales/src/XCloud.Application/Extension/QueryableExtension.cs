using System.Linq;

namespace XCloud.Application.Extension;

public static class QueryableExtension
{
    public static IQueryable<T> TakeUpTo5000<T>(this IQueryable<T> query)
    {
        query = query.Take(5000);
        return query;
    }
}