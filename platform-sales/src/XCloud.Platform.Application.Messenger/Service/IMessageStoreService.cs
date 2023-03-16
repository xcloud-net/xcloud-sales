using Volo.Abp.DependencyInjection;
using XCloud.Application.Service;
using XCloud.Platform.Application.Messenger.Redis;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Messenger.Service;

public interface IMessageStoreService : IXCloudApplicationService
{
    //
}

[ExposeServices(typeof(IMessageStoreService))]
public class RedisMessageStoreService : PlatformApplicationService, IMessageStoreService, ISingletonDependency
{
    private readonly MessengerRedisClient _messengerRedisClient;

    public RedisMessageStoreService(MessengerRedisClient messengerRedisClient)
    {
        _messengerRedisClient = messengerRedisClient;
    }
}