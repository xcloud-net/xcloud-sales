using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Auth.IdentityServer;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Api.Controller.Public;

[Route("/api/platform/user/auth")]
public class UserAuthController : PlatformBaseController, IUserController
{
    private readonly HttpClient _httpClient;
    private readonly IdentityServerAuthConfig _oAuthConfig;
    private readonly IWorkContext _workContext;

    public UserAuthController(
        IWorkContext<UserAuthController> workContext,
        IHttpClientFactory factory,
        IdentityServerAuthConfig oAuthConfig)
    {
        this._workContext = workContext;
        this._oAuthConfig = oAuthConfig;
        this._httpClient = factory.CreateClient("wx_login_");
    }

    [HttpPost("login-info")]
    public async Task<ApiResponse<SysUserDto>> LoginInfo()
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();
        return new ApiResponse<SysUserDto>().SetData(loginUser);
    }

    /// <summary>
    /// 密码授权模式
    /// </summary>
    [HttpPost("password-login")]
    public async Task<ApiResponse<TokenModel>> OauthPwdLogin([JsonData] PasswordLoginDto model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this._workContext.Configuration);
        var tokenResponse = await this._httpClient.RequestTokenAsync(new TokenRequest
        {
            Address = disco.TokenEndpoint,
            GrantType = "password",

            ClientId = this._oAuthConfig.ClientId,
            ClientSecret = this._oAuthConfig.ClientSecret,

            Parameters =
            {
                { "scope", this._oAuthConfig.Scope },
                { "username", model.IdentityName },
                { "password", model.Password }
            }
        });

        var res = tokenResponse.ToTokenModel();

        return res;
    }
}