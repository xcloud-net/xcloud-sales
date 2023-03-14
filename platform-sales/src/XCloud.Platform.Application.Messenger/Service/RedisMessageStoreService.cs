
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Redis;

namespace XCloud.Platform.Application.Messenger.Service;

[ExposeServices(typeof(IMessageStoreService))]
public class RedisMessageStoreService : IMessageStoreService, ISingletonDependency
{
    private readonly MessengerRedisClient _messengerRedisClient;

    public RedisMessageStoreService(MessengerRedisClient messengerRedisClient)
    {
        _messengerRedisClient = messengerRedisClient;
    }
}