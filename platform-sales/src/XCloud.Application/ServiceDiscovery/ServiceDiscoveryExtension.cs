using System.Threading.Tasks;
using XCloud.Core;

namespace XCloud.Application.ServiceDiscovery;

public static class ServiceDiscoveryExtension
{
    public static async Task<string> ResolveRequiredServiceAsync(this IServiceDiscoveryService serviceDiscoveryService,
        string name)
    {
        var service = await serviceDiscoveryService.ResolveServiceAsync(name);

        if (service.Empty)
            throw new ConfigException($"service:{name} is required");

        return service.Address;
    }
}