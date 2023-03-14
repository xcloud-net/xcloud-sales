
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Messenger.Service;

[ExposeServices(typeof(IMessageStoreService))]
public class RedisMessageStoreService : IMessageStoreService, ISingletonDependency
{
    //
}