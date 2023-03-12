using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace XCloud.Application.ServiceDiscovery;

public interface IServiceDiscoveryService
{
    Task<ServiceDiscoveryResponseDto> ResolveServiceAsync(string name);
}

public class ServiceDiscoveryService : IServiceDiscoveryService, IScopedDependency
{
    private readonly IServiceDiscoveryProvider[] _providers;

    public ServiceDiscoveryService(IServiceProvider serviceProvider)
    {
        this._providers = serviceProvider.GetServices<IServiceDiscoveryProvider>()
            .OrderByDescending(x => x.Order)
            .ToArray();
    }


    public async Task<ServiceDiscoveryResponseDto> ResolveServiceAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        foreach (var provider in this._providers)
        {
            var response = await provider.ResolveServiceOrNullAsync(name);
            if (response != null)
            {
                return response;
            }
        }

        return new ServiceDiscoveryResponseDto();
    }
}