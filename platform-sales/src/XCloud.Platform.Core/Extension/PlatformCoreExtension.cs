using System.Threading.Tasks;
using XCloud.Application.ServiceDiscovery;

namespace XCloud.Platform.Core.Extension;

public static class PlatformCoreExtension
{
    public static async Task<string> GetRequiredPublicGatewayAddressAsync(
        this IServiceDiscoveryService serviceDiscoveryService)
    {
        var response = await serviceDiscoveryService.ResolveRequiredServiceAsync("PublicGateway");
        return response;
    }

    public static async Task<string> GetRequiredInternalGatewayAddressAsync(
        this IServiceDiscoveryService serviceDiscoveryService)
    {
        var response = await serviceDiscoveryService.ResolveRequiredServiceAsync("InternalGateway");
        return response;
    }
}