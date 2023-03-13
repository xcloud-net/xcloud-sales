using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Application.ServiceDiscovery;

namespace XCloud.Platform.Core.Provider.ServiceDiscovery;

public class PlatformServiceAddressProvider : IServiceDiscoveryProvider, IScopedDependency
{
    public int Order => 1;

    public async Task<ServiceDiscoveryResponseDto> ResolveServiceOrNullAsync(string name)
    {
        await Task.CompletedTask;

        return null;
    }
}