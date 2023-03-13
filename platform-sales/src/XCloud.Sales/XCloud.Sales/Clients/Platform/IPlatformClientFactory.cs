using System.Net.Http;
using Nito.AsyncEx;
using XCloud.Application.ServiceDiscovery;
using XCloud.Platform.Core.Extension;

namespace XCloud.Sales.Clients.Platform;

public interface IPlatformClientFactory
{
    Task<HttpClient> CreateClientAsync();
}

[ExposeServices(typeof(IPlatformClientFactory))]
public class DefaultPlatformClientFactory : IPlatformClientFactory, ITransientDependency
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncLazy<HttpClient> _lazyHttpClient;
    private readonly IServiceDiscoveryService _serviceDiscoveryService;

    private async Task<HttpClient> RealCreateClientAsync()
    {
        await Task.CompletedTask;
        
        var baseUrl = await this._serviceDiscoveryService.GetRequiredInternalGatewayAddressAsync();
        var client = _httpClientFactory.CreateClient(nameof(DefaultPlatformClientFactory));
        client.BaseAddress = new Uri(baseUrl);
        
        return client;
    }

    /// <summary>
    ///    Each call to System.Net.Http.IHttpClientFactory.CreateClient(System.String) is
    //     guaranteed to return a new System.Net.Http.HttpClient instance. It is generally
    //     not necessary to dispose of the System.Net.Http.HttpClient as the System.Net.Http.IHttpClientFactory
    //     tracks and disposes resources used by the System.Net.Http.HttpClient.
    //     Callers are also free to mutate the returned System.Net.Http.HttpClient instance's
    //     public properties as desired.
    /// </summary>
    public async Task<HttpClient> CreateClientAsync()
    {
        var client = await _lazyHttpClient.Task;
        return client;
    }

    public DefaultPlatformClientFactory(IHttpClientFactory httpClientFactory, IServiceDiscoveryService serviceDiscoveryService)
    {
        this._httpClientFactory = httpClientFactory;
        _serviceDiscoveryService = serviceDiscoveryService;
        _lazyHttpClient = new AsyncLazy<HttpClient>(RealCreateClientAsync);
    }
}