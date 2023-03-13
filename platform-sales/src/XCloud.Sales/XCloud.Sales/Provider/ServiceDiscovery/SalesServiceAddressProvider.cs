using XCloud.Application.ServiceDiscovery;

namespace XCloud.Sales.Provider.ServiceDiscovery;

public class SalesServiceAddressProvider : IServiceDiscoveryProvider, IScopedDependency
{
    public int Order => default;

    public async Task<ServiceDiscoveryResponseDto> ResolveServiceOrNullAsync(string name)
    {
        await Task.CompletedTask;

        return null;
    }
}