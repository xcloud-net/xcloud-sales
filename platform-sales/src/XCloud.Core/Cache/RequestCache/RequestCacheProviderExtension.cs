using System.Threading.Tasks;
using XCloud.Core.Dto;

namespace XCloud.Core.Cache.RequestCache;

public static class RequestCacheProviderExtension
{
    public static async Task<T> GetOrSetAsync<T>(this IRequestCacheProvider holder, string key, Func<Task<T>> func)
    {
        if (holder == null)
            throw new ArgumentNullException(nameof(holder));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
            
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        try
        {
            var res = holder.GetObject<T>(key);
            if (res.IsSuccess())
            {
                return res.Data;
            }

            var data = await func.Invoke();
            holder.SetObject(key, data);

            return data;
        }
        catch
        {
            holder.RemoveKey(key);
            throw;
        }
    }
}