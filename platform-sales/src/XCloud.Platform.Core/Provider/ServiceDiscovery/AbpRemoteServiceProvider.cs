using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using XCloud.Application.ServiceDiscovery;
using XCloud.Core.Dto;

namespace XCloud.Platform.Core.Provider.ServiceDiscovery;

public class AbpRemoteServiceProvider : IServiceDiscoveryProvider, IScopedDependency
{
    private readonly IRemoteServiceConfigurationProvider _remoteServiceConfigurationProvider;

    public AbpRemoteServiceProvider(IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider)
    {
        _remoteServiceConfigurationProvider = remoteServiceConfigurationProvider;
    }

    public int Order => default;

    public async Task<ServiceDiscoveryResponseDto> ResolveServiceOrNullAsync(string name)
    {
        var response = await this._remoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync(name);

        if (response != null)
        {
            var service = new ServiceDiscoveryResponseDto();
            service.SetData(response.BaseUrl);
            return service;
        }

        return null;
    }
}