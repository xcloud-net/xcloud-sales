using StackExchange.Redis;
using Volo.Abp.DependencyInjection;
using XCloud.Redis;

namespace XCloud.Platform.Common.Application.Service.Messenger;

public interface IRedisDatabaseSelector
{
    IDatabase Database { get; }
    
    IConnectionMultiplexer Connection { get; }
}

[ExposeServices(typeof(IRedisDatabaseSelector))]
public class RedisDatabaseSelector : IRedisDatabaseSelector, ISingletonDependency
{
    public RedisDatabaseSelector(RedisClient redisClient)
    {
        var db = (int)RedisConsts.PubSub;
        this.Connection = redisClient.Connection;
        this.Database = redisClient.Connection.GetDatabase(db);
    }

    public IDatabase Database { get; }
    public IConnectionMultiplexer Connection { get; }
}