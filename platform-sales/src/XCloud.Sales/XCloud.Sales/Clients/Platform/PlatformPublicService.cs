using System.Net.Http;
using System.Net.Http.Headers;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Sales.Application;

namespace XCloud.Sales.Clients.Platform;

[ExposeServices(typeof(PlatformPublicService))]
public class PlatformPublicService : SalesAppService
{
    private readonly IPlatformClientFactory _platformClientFactory;
    public PlatformPublicService(IPlatformClientFactory platformClientFactory)
    {
        this._platformClientFactory = platformClientFactory;
    }

    public async Task<ApiResponse<SysUserDto>> GetCurrentLoginUserAsync(string token)
    {
        var client = await _platformClientFactory.CreateClientAsync();

        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/platform/user/auth/login-info");
        message.Content = null;
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(message);
        //unauthorized will return 401
        //response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var model = JsonDataSerializer.DeserializeFromString<ApiResponse<SysUserDto>>(json);

        return model;
    }
}