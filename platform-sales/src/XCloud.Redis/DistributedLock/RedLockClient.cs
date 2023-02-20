using Microsoft.Extensions.Logging;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Collections.Generic;
using XCloud.Core.DependencyInjection.ServiceWrapper;

namespace XCloud.Redis.DistributedLock;

public class RedLockClient : ISingleInstanceService
{
    /// <summary>
    /// string resource, 锁定对象
    /// TimeSpan expiryTime,锁过期时间 
    /// TimeSpan waitTime, 最长等待锁时间
    /// TimeSpan retryTime,重试间隔
    /// </summary>
    public RedLockFactory RedLockFactory { get; }

    public RedLockClient(RedisClient redisClient, ILoggerFactory loggerFactory)
    {
        var multiplexers = new List<RedLockMultiplexer>
        {
            new RedLockMultiplexer(redisClient.Connection)
        };

        foreach (var m in multiplexers)
        {
            m.RedisDatabase = (int)RedisConsts.DistributedLock;
        }

        this.RedLockFactory = RedLockFactory.Create(multiplexers, loggerFactory);
    }

    public int DisposeOrder => default;

    public void Dispose()
    {
        this.RedLockFactory?.Dispose();
    }
}