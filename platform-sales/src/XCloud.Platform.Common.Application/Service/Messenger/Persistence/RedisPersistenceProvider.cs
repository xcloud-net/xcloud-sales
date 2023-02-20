
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Common.Application.Service.Messenger.Persistence;

[ExposeServices(typeof(IPersistenceProvider))]
public class RedisPersistenceProvider : IPersistenceProvider, ISingletonDependency
{
    //
}