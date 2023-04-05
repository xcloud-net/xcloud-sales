using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Auth.Configuration;
using XCloud.Platform.Auth.IdentityServer;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller;

[Route("/api/platform/auth")]
public class AuthController : PlatformBaseController
{
    private readonly HttpClient _httpClient;
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public AuthController(
        IHttpClientFactory factory, 
        IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
        this._httpClient = factory.CreateClient("identity");
    }

    [HttpPost("empty")]
    public Task<string> Empty() => Task.FromResult(this.Clock.Now.ToString());

    /// <summary>
    /// 刷新token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ApiResponse<AuthTokenDto>> RefreshToken([JsonData] RefreshTokenDto model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var identityServerAuthConfig = this.Configuration.GetOAuthServerOption();

        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this.Configuration);

        var tokenResponse = await this._httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest()
        {
            Address = disco.TokenEndpoint,
            RefreshToken = model.RefreshToken,
            GrantType = "refresh_token",
            ClientId = identityServerAuthConfig.ClientId,
            ClientSecret = identityServerAuthConfig.ClientSecret,
            Scope = identityServerAuthConfig.Scope,
        });

        var res = tokenResponse.ToAuthTokenDto();
        res.ThrowIfErrorOccured();

        return res;
    }

    [Obsolete]
    [HttpPost(nameof(authorization_test))]
    public async Task<string> authorization_test([FromServices] IAuthorizationService authorizationService)
    {
        var res = await authorizationService.AuthorizeAsync(null, new AdminPermissionRequirement()
        {
            Permissions = new[] { "delete-user" }
        });
        return "123";
    }

    /// <summary>
    /// 生成jwt
    /// </summary>
    /// <returns></returns>
    [Obsolete]
    [HttpPost(nameof(test_jwt))]
    public ApiResponse<object> test_jwt()
    {
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Subject, "wer")
        };

        //这个secret key也可以通过jwks来生成，不需要保密。只要保持一致就可以了
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456789123456789123456789"));
        var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "http://localhost:5000",
            audience: "http://localhost:5000",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: sign);

        return new ApiResponse<object>().SetData(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [Obsolete]
    [HttpPost(nameof(test_jwt_xx))]
    public async Task<ApiResponse<object>> test_jwt_xx()
    {
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Subject, "wer")
        };
        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this.Configuration);
        using var response = await this._httpClient.GetAsync(disco.JwksUri);
        var data = await response.Content.ReadAsStringAsync();
        var keys = JObject.Parse(data)["keys"];
        var k = keys[0].ToString();

        //这个secret key也可以通过jwks来生成，不需要保密。只要保持一致就可以了
        var key = new JsonWebKey(k);
        var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "http://localhost:5000",
            audience: "http://localhost:5000",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: sign);

        return new ApiResponse<object>().SetData(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}