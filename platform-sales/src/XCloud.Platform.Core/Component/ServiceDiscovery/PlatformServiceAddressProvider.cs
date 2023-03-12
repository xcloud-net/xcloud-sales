using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using XCloud.Application.ServiceDiscovery;
using XCloud.Core.Dto;
using XCloud.Platform.Shared.Settings;

namespace XCloud.Platform.Core.Component.ServiceDiscovery;

public class PlatformServiceAddressProvider : IServiceDiscoveryProvider, IScopedDependency
{
    private readonly IOptions<PlatformServiceAddressOption> _options;

    public PlatformServiceAddressProvider(IOptions<PlatformServiceAddressOption> options)
    {
        _options = options;
    }

    public int Order => 1;

    public async Task<ServiceDiscoveryResponseDto> ResolveServiceOrNullAsync(string name)
    {
        await Task.CompletedTask;

        if (name == nameof(this._options.Value.InternalGateway))
        {
            var service = new ServiceDiscoveryResponseDto();
            service.SetData(this._options.Value.InternalGateway);
            return service;
        }
        else if (name == nameof(this._options.Value.PublicGateway))
        {
            var service = new ServiceDiscoveryResponseDto();
            service.SetData(this._options.Value.PublicGateway);
            return service;
        }
        else if (name == nameof(this._options.Value.FrontEnd))
        {
            var service = new ServiceDiscoveryResponseDto();
            service.SetData(this._options.Value.FrontEnd);
            return service;
        }

        return null;
    }
}