using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace XCloud.Core.Http.Dynamic;

public interface IDynamicClientServiceDiscovery
{
    string GetServiceAddress(string service_name);
}

internal class ServiceDiscoveryViaConfiguration : IDynamicClientServiceDiscovery
{
    private readonly IConfiguration configuration;
    public ServiceDiscoveryViaConfiguration(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GetServiceAddress(string service_name)
    {
        service_name.Should().NotBeNullOrEmpty();
        var res = configuration[$"RemoteServices:{service_name}:BaseUrl"];
        return res;
    }
}