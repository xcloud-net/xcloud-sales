using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Configuration;

namespace XCloud.Platform.Auth.IdentityServer;

public static class IdentityServerExtension
{
    
    public static string GetIdentityServerAddressOrThrow(this IConfiguration config)
    {
        var option = config.GetOAuthServerOption();

        var identityServer = option.Server;
        if (string.IsNullOrWhiteSpace(identityServer))
            throw new ArgumentNullException($"请配置{nameof(identityServer)}");

        if (identityServer.EndsWith("/"))
            throw new ArgumentException("identity server后面不要加斜杠");

        return identityServer;
    }

    public static async Task<DiscoveryDocumentResponse> GetIdentityServerDiscoveryDocuments(this HttpClient httpClient,
        IConfiguration config)
    {
        var identityServer = config.GetIdentityServerAddressOrThrow();

        var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
        {
            Address = identityServer,
            Policy = new DiscoveryPolicy() { RequireHttps = false, }
        });

        if (disco.IsError)
            throw new UserFriendlyException(disco.Error);

        return disco;
    }

    public static ApiResponse<AuthTokenDto> ToAuthTokenDto(this TokenResponse tokenResponse)
    {
        var res = new ApiResponse<AuthTokenDto>();

        if (tokenResponse.IsError)
        {
            var err = tokenResponse.ErrorDescription ?? tokenResponse.Error ?? "登陆失败";
            return res.SetError(err);
        }

        var model = new AuthTokenDto()
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiredTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
        };

        return res.SetData(model);
    }

}