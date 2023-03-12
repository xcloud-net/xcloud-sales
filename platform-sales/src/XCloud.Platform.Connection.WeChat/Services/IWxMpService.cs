using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using Volo.Abp.Application.Services;
using XCloud.Core.Application;
using XCloud.Platform.Common.Application.Extension;
using XCloud.Platform.Common.Application.Service.Storage;
using XCloud.Platform.Connection.WeChat.Configuration;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Redis;
using XCloud.Redis.DistributedLock;

namespace XCloud.Platform.Connection.WeChat.Services;

public interface IWxMpService : IApplicationService
{
    string PlatformName { get; }

    Task TryUpdateUserProfileFromWechat(string userId, WechatMpOption config, string accessToken, string openId);

    Task<SnsUserInfoResponse> GetUserWechatProfileAsync(WechatMpOption config, string accessToken, string openId);

    Task<string> GetOrRefreshClientAccessTokenAsync();

    Task<SnsOAuth2AccessTokenResponse> GetUserAccessTokenAsync(string code);

    Task<CgibinTokenResponse> GetClientAccessTokenAsync();
}

public class WxMpService : XCloudApplicationService, IWxMpService
{
    private readonly IExternalAccessTokenService _externalAccessTokenService;
    private readonly RedLockClient _redLockClient;
    private readonly IUserProfileService _userProfileService;
    private readonly IStorageService _storageService;
    private readonly HttpClient _httpClient;

    public WxMpService(IHttpClientFactory httpClientFactory,
        IStorageService storageService,
        IUserProfileService userProfileService,
        IExternalAccessTokenService userExternalAccessTokenService,
        RedLockClient redLockClient)
    {
        this._userProfileService = userProfileService;
        this._storageService = storageService;
        this._externalAccessTokenService = userExternalAccessTokenService;
        this._redLockClient = redLockClient;
        this._httpClient = httpClientFactory.CreateClient(nameof(WxMpService));
    }

    public string PlatformName => ThirdPartyPlatforms.WxMp;

    public async Task<string> GetOrRefreshClientAccessTokenAsync()
    {
        var config = this.Configuration.GetWxMpConfig();

        var token = await _externalAccessTokenService.GetValidClientAccessTokenAsync(new GetValidClientAccessTokenInput()
        {
            AppId = config.AppId,
            Platform = PlatformName
        });
        var now = Clock.Now;

        if (token == null || token.ExpiredAt < now.AddMinutes(5))
        {
            var lockKey = $"{nameof(WxMpService)}.{nameof(GetOrRefreshClientAccessTokenAsync)}";

            using var dlock = await _redLockClient.RedLockFactory.CreateLockAsync(
                resource: lockKey,
                expiryTime: TimeSpan.FromSeconds(5));

            if (dlock.IsAcquired)
            {
                var accessToken = new SysUserExternalAccessTokenDto()
                {
                    AppId = config.AppId,
                    Platform = PlatformName,
                    GrantType = "client",
                    AccessTokenType = (int)AccessTokenTypeEnum.Client
                };
                var tokenResponse = await GetClientAccessTokenAsync();
                accessToken.AccessToken = tokenResponse.AccessToken;
                accessToken.RefreshToken = string.Empty;
                accessToken.ExpiredAt = now.AddSeconds(tokenResponse.ExpiresIn);

                await _externalAccessTokenService.InsertAccessTokenAsync(accessToken);

                return accessToken.AccessToken;
            }
            else
            {
                throw new FailToGetRedLockException("can't get dlock");
            }
        }

        return token.AccessToken;
    }

    public async Task<CgibinTokenResponse> GetClientAccessTokenAsync()
    {
        var config = this.Configuration.GetWxMpConfig();

        var wechatOption = new WechatApiClientOptions()
        {
            AppId = config.AppId,
            AppSecret = config.AppSecret
        };

        var client = new WechatApiClient(wechatOption);

        var response = await client.ExecuteCgibinTokenAsync(new CgibinTokenRequest() { });

        if (!response.IsSuccessful())
            throw new WechatException(nameof(GetClientAccessTokenAsync), response);

        return response;
    }

    public async Task<SnsOAuth2AccessTokenResponse> GetUserAccessTokenAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        var config = this.Configuration.GetWxMpConfig();

        var wechatOption = new WechatApiClientOptions()
        {
            AppId = config.AppId,
            AppSecret = config.AppSecret
        };

        var client = new WechatApiClient(wechatOption);

        var response = await client.ExecuteSnsOAuth2AccessTokenAsync(new SnsOAuth2AccessTokenRequest()
        {
            Code = code
        });

        if (!response.IsSuccessful())
            throw new WechatException(nameof(GetUserAccessTokenAsync), response);

        return response;
    }

    public async Task<SnsUserInfoResponse> GetUserWechatProfileAsync(WechatMpOption config, string accessToken, string openId)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(openId))
            throw new ArgumentNullException(nameof(GetUserWechatProfileAsync));

        var wechatOption = new WechatApiClientOptions()
        {
            AppId = config.AppId,
            AppSecret = config.AppSecret
        };

        var client = new WechatApiClient(wechatOption);

        var response = await client.ExecuteSnsUserInfoAsync(new SnsUserInfoRequest()
        {
            AccessToken = accessToken,
            OpenId = openId
        });

        if (!response.IsSuccessful())
            throw new WechatException(nameof(GetUserAccessTokenAsync), response);

        return response;
    }

    public async Task TryUpdateUserProfileFromWechat(string userId, WechatMpOption config, string accessToken, string openId)
    {
        var response = await this.GetUserWechatProfileAsync(config, accessToken, openId);

        if (!string.IsNullOrWhiteSpace(response.Nickname))
        {
            try
            {
                await this._userProfileService.UpdateNickNameAsync(userId, response.Nickname);
            }
            catch (Exception e)
            {
                this.Logger.LogError(message: e.Message, exception: e);
            }
        }

        if (!string.IsNullOrWhiteSpace(response.HeadImageUrl))
        {
            try
            {
                var resourseMeta = await this._storageService.UploadFromUrlAsync(this._httpClient,
                    new FileUploadFromUrl() { Url = response.HeadImageUrl, FileName = "wechat-avatar.png" });

                var dto = resourseMeta.ToDto();

                var jsondata = this.JsonDataSerializer.SerializeToString(dto);

                await this._userProfileService.UpdateAvatarAsync(userId, jsondata);
            }
            catch (Exception e)
            {
                this.Logger.LogError(message: e.Message, exception: e);
            }
        }
    }
}