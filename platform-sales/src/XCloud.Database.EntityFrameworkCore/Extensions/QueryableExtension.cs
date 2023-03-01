using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Dto;

namespace XCloud.Database.EntityFrameworkCore.Extensions;

public static class QueryableExtension
{
    public static async Task<int> CountOrDefaultAsync<T>(this IQueryable<T> query, PagedRequest request)
    {
        var count = default(int);

        if (!request.SkipCalculateTotalCount)
            count = await query.CountAsync();

        return count;
    }
}