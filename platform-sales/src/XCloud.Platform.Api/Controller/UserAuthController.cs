using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Auth.Configuration;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller;

[Route("/api/platform/user/auth")]
public class UserAuthController : PlatformBaseController, IUserController
{
    private readonly HttpClient _httpClient;
    private readonly IWorkContext _workContext;

    public UserAuthController(
        IWorkContext<UserAuthController> workContext,
        IHttpClientFactory factory)
    {
        this._workContext = workContext;
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

        var oAuthConfig = this.Configuration.GetOAuthServerOption();

        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this._workContext.Configuration);
        var tokenResponse = await this._httpClient.RequestTokenAsync(new TokenRequest
        {
            Address = disco.TokenEndpoint,
            GrantType = "password",

            ClientId = oAuthConfig.ClientId,
            ClientSecret = oAuthConfig.ClientSecret,

            Parameters =
            {
                { "scope", oAuthConfig.Scope },
                { "username", model.IdentityName },
                { "password", model.Password }
            }
        });

        var res = tokenResponse.ToTokenModel();

        return res;
    }
}