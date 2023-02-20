using System.Threading.Tasks;

namespace XCloud.Core.Cache;

public static class CachePolicyExtension
{
    public static async Task<T> ExecuteWithPolicyAsync<T>(this ICacheProvider provider, Func<Task<T>> source, CacheOption<T> option, CachePolicy policy)
    {
        if (policy.Cache ?? false)
        {
            return await provider.GetOrSetAsync(source, option);
        }
        else if (policy.CacheOnly ?? false)
        {
            var result = await provider.GetAsync<T>(option.Key);
            return result.Data;
        }
        else if (policy.Refresh ?? false)
        {
            return await provider.RefreshCacheAsync(source, option);
        }
        else if (policy.RemoveCache ?? false)
        {
            await provider.RemoveAsync(option.Key);
            return await Task.FromResult<T>(default);
        }
        else if (policy.Source ?? false)
        {
            return await source.Invoke();
        }

        throw new NotSupportedException(nameof(policy));
    }
}