using Microsoft.Extensions.Options;
using XCloud.Application.ServiceDiscovery;
using XCloud.Core.Dto;
using XCloud.Sales.Core.Settings;

namespace XCloud.Sales.Component.ServiceDiscovery;

public class SalesServiceAddressProvider : IServiceDiscoveryProvider, IScopedDependency
{
    private readonly IOptions<SalesServiceAddressOption> _options;

    public SalesServiceAddressProvider(IOptions<SalesServiceAddressOption> options)
    {
        _options = options;
    }

    public int Order => default;

    public async Task<ServiceDiscoveryResponseDto> ResolveServiceOrNullAsync(string name)
    {
        await Task.CompletedTask;
        
        if (name == nameof(this._options.Value.FrontEnd))
        {
            var service = new ServiceDiscoveryResponseDto();
            service.SetData(this._options.Value.FrontEnd);
            return service;
        }

        return null;
    }
}