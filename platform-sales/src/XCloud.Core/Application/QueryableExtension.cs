namespace XCloud.Core.Application;

public static class QueryableExtension
{
    public static IQueryable<T> TakeUpTo5000<T>(this IQueryable<T> query)
    {
        query = query.Take(5000);
        return query;
    }
}