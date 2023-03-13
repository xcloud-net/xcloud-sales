
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Messenger.Service;

[ExposeServices(typeof(IPersistenceProvider))]
public class RedisPersistenceProvider : IPersistenceProvider, ISingletonDependency
{
    //
}