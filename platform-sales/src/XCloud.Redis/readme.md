
# 查找附近的人
```csharp
            var db = con.Connection.GetDatabase((int)RedisConsts.KVStore);

            var now = DateTime.UtcNow;

            db.SetAdd("set", "dd" + now);

            db.SortedSetAdd("sorted_set", "d" + now, now.Ticks);
            db.SortedSetAdd("sorted_set", "d3" + now, now.Ticks);

            db.HashSet("hash", "a" + now, now.ToString());

            //db.GeoAdd("position", 43, 54, "userid");
            //db.GeoRadius("position", "userid", 4, count: 100);
            //var res = db.GeoRadius("position", 4, 5, 7);

            /*
             redis计算“附近的人”。
             把地图分为很多的小方格，每个方格一个key。并且知道每个方格的中心点坐标。

                用我当前坐标查出我附近的方格。
                然后每个方格里查出离我最近的人。
                最后多个数据集聚合得出数据。

            数据上报的时候也是先查出我附近的方格。然后把我的位置写到附近的所有方格里

             */
```

# cache provider
```csharp
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using XCloud.Core.Cache;
using XCloud.Core.Cache.Models;

namespace XCloud.Redis.Cache
{
    public class MyRedisCacheProvider : ICacheProvider
    {
        private readonly IDatabase _db;
        private readonly CacheSerializer dataSerializer;

        public ILogger Logger { get; }
        public CacheProviderOption Option { get; }

        public MyRedisCacheProvider(IServiceProvider provider,
            CacheSerializer dataSerializer,
            RedisClient con,
            ILogger<MyRedisCacheProvider> logger)
        {
            this._db = con.Connection.SelectDatabase((int)RedisConsts.Caching);
            this.dataSerializer = dataSerializer;

            this.Logger = logger;
            this.Option = new CacheProviderOption(provider.ResolveConfiguration());
        }

        #region Methods

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public bool IsSet(string key)
        {
            key.Should().NotBeNullOrEmpty();
            var res = this._db.KeyExists(key);
            return res;
        }

        public async Task<bool> IsSetAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();
            var res = await this._db.KeyExistsAsync(key);
            return res;
        }

        public void Remove(string key)
        {
            key.Should().NotBeNullOrEmpty();
            this._db.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();
            await this._db.KeyDeleteAsync(key);
        }

        public CacheResult<T> Get<T>(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var rValue = this._db.StringGet(key);
            if (rValue.HasValue)
            {
                var res = this.dataSerializer.DeserializeCacheResultFromString<T>(rValue);
                return res;
            }

            return new CacheResult<T>();
        }

        public void Set<T>(string key, T data, TimeSpan? expire)
        {
            key.Should().NotBeNullOrEmpty();

            var json = this.dataSerializer.SerializeCacheResultToString(new CacheResult<object>(data));

            this._db.StringSet(key, (string)json, expire);
        }

        public async Task<CacheResult<T>> GetAsync<T>(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var rValue = await this._db.StringGetAsync(key);
            if (rValue.HasValue)
            {
                var res = this.dataSerializer.DeserializeCacheResultFromString<T>(rValue);
                return res;
            }

            return new CacheResult<T>();
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? expire)
        {
            key.Should().NotBeNullOrEmpty();

            var json = this.dataSerializer.SerializeCacheResultToString(new CacheResult<object>(data));

            await this._db.StringSetAsync(key, (string)json, expire);
        }

        #endregion
    }
}
```