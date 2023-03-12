using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Volo.Abp;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Auth.IdentityServer;
using XCloud.Platform.Common.Application.Service.Token;
using XCloud.Platform.Connection.WeChat.Configuration;
using XCloud.Platform.Connection.WeChat.Services;
using XCloud.Platform.Connection.WeChat.Services.Mp;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Platform.Shared;

namespace XCloud.Platform.Api.Controller;

[Route("/api/platform/user/wx-auth")]
public class UserWxAuthController : PlatformBaseController, IUserController
{
    private readonly HttpClient _httpClient;
    private readonly IdentityServerAuthConfig _oAuthConfig;
    private readonly IWorkContext _workContext;
    private readonly IWxMpService _wxMpService;
    private readonly IExternalConnectService _externalConnectService;
    private readonly IUserAccountService _userAccountService;
    private readonly TempCodeService _tempCodeService;
    private readonly IConfiguration _configuration;
    private readonly IWxUnionService _wxUnionService;

    public UserWxAuthController(
        IConfiguration configuration,
        TempCodeService tempCodeService,
        IUserAccountService userAccountService,
        IExternalConnectService userExternalAccountService,
        IWxMpService wxMpService,
        IWorkContext<UserAuthController> workContext,
        IHttpClientFactory factory,
        IdentityServerAuthConfig oAuthConfig,
        IWxUnionService wxUnionService)
    {
        this._configuration = configuration;
        this._tempCodeService = tempCodeService;
        this._userAccountService = userAccountService;
        this._externalConnectService = userExternalAccountService;
        this._wxMpService = wxMpService;
        this._workContext = workContext;
        this._oAuthConfig = oAuthConfig;
        _wxUnionService = wxUnionService;
        this._httpClient = factory.CreateClient("wx_login_");
    }

    [HttpPost("wx-mp-code-auth")]
    public async Task<ApiResponse<TokenModel>> WxMpCodeAuth([FromBody] OAuthCodeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentNullException(nameof(dto.Code));

        var wxToken = await this._wxMpService.GetUserAccessTokenAsync(dto.Code);
        if (string.IsNullOrWhiteSpace(wxToken.OpenId))
            throw new BusinessException("empty openid from wechat mp");

        var config = this._configuration.GetWxMpConfig();

        var entity = await this._externalConnectService.FindByOpenIdAsync(
            ThirdPartyPlatforms.WxMp, config.AppId, wxToken.OpenId);

        if (entity == null)
        {
            var randomUserName = this.GuidGenerator.CreateGuidString();
            var createUserResponse =
                await this._userAccountService.CreateUserAccountAsync(new IdentityNameDto(randomUserName));
            createUserResponse.ThrowIfErrorOccured();

            entity = new SysExternalConnect
            {
                Platform = ThirdPartyPlatforms.WxMp,
                AppId = config.AppId,
                UserId = createUserResponse.Data.Id,
                OpenId = wxToken.OpenId
            };

            await this._externalConnectService.SaveAsync(entity);
            dto.UseWechatProfile = true;
        }

        //create connection if needed
        if (!string.IsNullOrWhiteSpace(wxToken.UnionId))
        {
            //save openid-union id mapping
            await this._wxUnionService.SaveOpenIdUnionIdMappingAsync(entity.Platform, entity.AppId,
                entity.OpenId, wxToken.UnionId);
        }

        var userId = entity.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            throw new BusinessException("failed to get userid");

        var user = await this._userAccountService.GetUserDtoByIdAsync(userId);

        if (user == null)
            throw new UserFriendlyException("the account connected to this wechat is invalid");

        if (dto.UseWechatProfile ?? false)
        {
            //try sync wechat user profile
            await this._wxMpService.TryUpdateUserProfileFromWechat(user.Id, config, wxToken.AccessToken,
                wxToken.OpenId);
        }

        //create temp key
        var tempCode = await this._tempCodeService.CreateTempCode(new IdDto(userId));

        //create access token
        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this._workContext.Configuration);
        var tokenResponse = await this._httpClient.RequestTokenAsync(new TokenRequest
        {
            Address = disco.TokenEndpoint,
            GrantType = IdentityConsts.GrantType.InternalGrantType,

            ClientId = this._oAuthConfig.ClientId,
            ClientSecret = this._oAuthConfig.ClientSecret,

            Parameters =
            {
                { "key", tempCode },
                { "id", userId }
            }
        });

        var res = tokenResponse.ToTokenModel();
        res.ThrowIfErrorOccured();

        return res;
    }

    /// <summary>
    /// 微信第三方登陆
    /// </summary>
    [HttpPost("code-login")]
    [Obsolete]
    public async Task<ApiResponse<TokenModel>> WxOauthLogin([JsonData] UserCodeLoginDto model)
    {
        model.Should().NotBeNull();

        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this._workContext.Configuration);
        var tokenResponse = await this._httpClient.RequestTokenAsync(new TokenRequest
        {
            Address = disco.TokenEndpoint,
            GrantType = IdentityConsts.GrantType.UserWechat,

            ClientId = this._oAuthConfig.ClientId,
            ClientSecret = this._oAuthConfig.ClientSecret,

            Parameters =
            {
                { "scope", this._oAuthConfig.Scope },
                { "code", model.Code },
                { "nick_name", model.NickName },
                { "avatar_url", model.AvatarUrl }
            }
        });

        var res = tokenResponse.ToTokenModel();
        res.ThrowIfErrorOccured();

        return res;
    }
}