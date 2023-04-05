using System.Threading.Tasks;

namespace XCloud.Application.ServiceDiscovery;

public interface IServiceDiscoveryProvider
{
    int Order { get; }
    
    Task<ServiceDiscoveryResponseDto> ResolveServiceOrNullAsync(string name);
}