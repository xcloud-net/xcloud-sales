namespace XCloud.Core.Helper;

public static class PageHelper
{
    public static int GetPagedSkip(int page, int pageSize)
    {
        EnsurePage(page);
        EnsurePageSize(pageSize);

        var skip = (page - 1) * pageSize;

        return skip;
    }
    
    public static void EnsurePage(int page)
    {
        if (page <= 0)
            throw new ArgumentException(nameof(page));
    }

    public static void EnsurePageSize(int pageSize)
    {
        if (pageSize <= 0)
            throw new ArgumentException(nameof(pageSize));
    }

    public static void EnsureMaxPageSize(int pageSize, int maxPageSize)
    {
        if (pageSize > maxPageSize)
            throw new ArgumentException(nameof(EnsureMaxPageSize));
    }
}