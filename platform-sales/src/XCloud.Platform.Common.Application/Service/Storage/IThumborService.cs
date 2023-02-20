using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using XCloud.Core.Application;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Shared;

namespace XCloud.Platform.Common.Application.Service.Storage;

public interface IThumborService : IXCloudApplicationService
{
    /// <summary>
    /// load data to memory
    /// in this way,may cause out of memory
    /// </summary>
    /// <param name="url"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    Task<byte[]> ResizeOrNullAsync(string url, int height, int width);

    /// <summary>
    /// dispose stream after use
    /// </summary>
    /// <param name="url"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    Task<Stream> GetResizedStreamOrNullAsync(string url, int height, int width);
    
    /// <summary>
    /// copy data to target stream
    /// </summary>
    /// <param name="url"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="outputStream"></param>
    /// <returns></returns>
    Task<bool> ResizeAndWriteToStreamAsync(string url, int height, int width, Stream outputStream);
}

[ExposeServices(typeof(IThumborService))]
public class ThumborService : PlatformApplicationService, IThumborService, IScopedDependency
{
    private readonly HttpClient _httpClient;
    private readonly IRemoteServiceConfigurationProvider _remoteServiceConfigurationProvider;

    public ThumborService(IHttpClientFactory httpClientFactory,
        IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider)
    {
        this._httpClient = httpClientFactory.CreateClient(nameof(ThumborService));
        this._remoteServiceConfigurationProvider = remoteServiceConfigurationProvider;
    }

    /// <summary>
    /// https://thumbor.readthedocs.io/en/latest/getting_started.html#changing-its-size
    /// </summary>
    /// <param name="url"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    public async Task<byte[]> ResizeOrNullAsync(string url, int height, int width)
    {
        using var ms = new MemoryStream();

        if (await this.ResizeAndWriteToStreamAsync(url, height, width, ms))
        {
            var bs = ms.ToArray();
            return bs;
        }

        return null;
    }

    public async Task<Stream> GetResizedStreamOrNullAsync(string url, int height, int width)
    {
        var gateway = await this._remoteServiceConfigurationProvider.ResolveGatewayBaseAddressAsync();
        url = HttpUtility.UrlEncode(url);

        //http://localhost:7070/unsafe/900x200/https://t7.baidu.com/it/u=2621658848,3952322712&fm=193&f=GIF
        var finalUrl = $"{gateway}/internal-api/thumbor/unsafe/{width}x{height}/{url}";
        
        var response = await this._httpClient.GetAsync(finalUrl);

        if (!response.IsSuccessStatusCode)
        {
            using (response)
            {
                //
            }

            return null;
        }
        
        var stream = await response.Content.ReadAsStreamAsync();

        return stream;
    }
    
    public async Task<bool> ResizeAndWriteToStreamAsync(string url, int height, int width,Stream outputStream)
    {
        var gateway = await this._remoteServiceConfigurationProvider.ResolveGatewayBaseAddressAsync();
        url = HttpUtility.UrlEncode(url);

        //http://localhost:7070/unsafe/900x200/https://t7.baidu.com/it/u=2621658848,3952322712&fm=193&f=GIF
        var finalUrl = $"{gateway}/internal-api/thumbor/unsafe/{width}x{height}/{url}";

        using var response = await this._httpClient.GetAsync(finalUrl);

        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            await stream.CopyToAsync(outputStream);
            return true;
        }

        return false;
    }
}