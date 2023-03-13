
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Common.Service.Messenger.Persistence;

[ExposeServices(typeof(IPersistenceProvider))]
public class RedisPersistenceProvider : IPersistenceProvider, ISingletonDependency
{
    //
}