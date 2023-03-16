using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Member.Service.User;

[ExposeServices(typeof(UserUtils))]
public class UserUtils : ISingletonDependency
{
}