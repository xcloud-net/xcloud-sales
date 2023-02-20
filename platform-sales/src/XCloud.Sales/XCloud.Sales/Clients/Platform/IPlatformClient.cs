using System.Net.Http;
using Nito.AsyncEx;
using Volo.Abp.Http.Client;
using XCloud.Platform.Shared;

namespace XCloud.Sales.Clients.Platform;

public interface IPlatformClient
{
    Task<HttpClient> CreateClientAsync();
}

[ExposeServices(typeof(IPlatformClient))]
public class DefaultPlatformClient : IPlatformClient, ITransientDependency
{
    private readonly IRemoteServiceConfigurationProvider _remoteServiceConfigurationProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncLazy<HttpClient> _lazyHttpClient;

    async Task<HttpClient> _CreateClientAsync()
    {
        var baseUrl = await _remoteServiceConfigurationProvider.ResolveGatewayBaseAddressAsync();
        var client = _httpClientFactory.CreateClient(nameof(DefaultPlatformClient));
        client.BaseAddress = new Uri(baseUrl);
        return client;
    }

    /// <summary>
    /// Each call to System.Net.Http.IHttpClientFactory.CreateClient(System.String) is
    //     guaranteed to return a new System.Net.Http.HttpClient instance. It is generally
    //     not necessary to dispose of the System.Net.Http.HttpClient as the System.Net.Http.IHttpClientFactory
    //     tracks and disposes resources used by the System.Net.Http.HttpClient.
    //     Callers are also free to mutate the returned System.Net.Http.HttpClient instance's
    //     public properties as desired.
    /// </summary>
    /// <returns></returns>
    public async Task<HttpClient> CreateClientAsync()
    {
        var client = await _lazyHttpClient.Task;
        return client;
    }

    public DefaultPlatformClient(
        IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider,
        IHttpClientFactory httpClientFactory)
    {
        _remoteServiceConfigurationProvider = remoteServiceConfigurationProvider;
        this._httpClientFactory = httpClientFactory;
        _lazyHttpClient = new AsyncLazy<HttpClient>(_CreateClientAsync);
    }
}