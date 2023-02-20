using System.Threading.Tasks;
using XCloud.Core.DataSerializer;
using XCloud.Core.Dto;

namespace XCloud.Core.Cache;

public static class CacheProviderExtension
{
    private static async Task<T> GetDataFromSourceAsync<T>(Func<Task<T>> source)
    {
        try
        {
            var data = await source.Invoke();
            return data;
        }
        catch (Exception e)
        {
            throw new CacheSourceException(e);
        }
    }

    public static async Task<T> RefreshCacheAsync<T>(this ICacheProvider provider, Func<Task<T>> source,
        CacheOption option)
    {
        var data = await source.Invoke();

        await provider.SetAsync(option.Key, data, option.Expiration);

        return data;
    }

    public static async Task<T> GetOrSetAsync<T>(this ICacheProvider provider,
        Func<Task<T>> source,
        CacheOption<T> option)
    {
        try
        {
            //默认都缓存
            var cacheCondition = option.CacheCondition ?? (x => true);

            var cacheResult = await provider.GetAsync<T>(option.Key);
            if (cacheResult.IsSuccess())
            {
                return cacheResult.Data;
            }

            var dataFromSource = await GetDataFromSourceAsync(source);

            //判断是否需要写入缓存
            var cacheEnabled = cacheCondition.Invoke(dataFromSource);
            if (cacheEnabled)
            {
                await provider.SetAsync(option.Key, dataFromSource, option.Expiration);
            }

            return dataFromSource;
        }
        catch (CacheSourceException e)
        {
            //溯源函数错误，无解，直接抛出
            if (e.InnerException != null)
                throw e.InnerException;
            throw;
        }
        catch (SerializeException e)
        {
            if (option.RemoveCacheKeyWhenSerializeException)
            {
                provider.Logger.LogError(message: e.Message, exception: e);
                //针对DeserializeException的异常处理还没想好
                return await RefreshCacheAsync(provider, source, option);
            }
            else
            {
                throw;
            }
        }
        catch (Exception e) when (!string.IsNullOrWhiteSpace(e.Message))
        {
            if (option.IgnoreCacheException)
            {
                var data = await source.Invoke();
                return data;
            }

            throw new CacheException($"读取缓存异常-缓存key:{option.Key}", e);
        }
    }
}