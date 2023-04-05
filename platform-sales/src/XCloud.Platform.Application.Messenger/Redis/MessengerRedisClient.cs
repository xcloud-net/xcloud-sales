using StackExchange.Redis;
using Volo.Abp.DependencyInjection;
using XCloud.Redis;

namespace XCloud.Platform.Application.Messenger.Redis;

[ExposeServices(typeof(MessengerRedisClient))]
public class MessengerRedisClient : ISingletonDependency
{
    public MessengerRedisClient(RedisClient redisClient, RedisKeyManager redisKeyManager)
    {
        RedisKeyManager = redisKeyManager;
        var db = (int)RedisConsts.PubSub;
        this.Connection = redisClient.Connection;
        this.Database = redisClient.Connection.GetDatabase(db);
    }

    public RedisKeyManager RedisKeyManager { get; }

    public IDatabase Database { get; }
    public IConnectionMultiplexer Connection { get; }
}