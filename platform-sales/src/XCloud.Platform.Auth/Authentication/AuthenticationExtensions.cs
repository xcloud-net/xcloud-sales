using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Platform.Auth.Application.Admin.Filter;
using XCloud.Platform.Auth.Configuration;

namespace XCloud.Platform.Auth.Authentication;

public static class AuthenticationExtensions
{
    public static bool IsLoginRequired(this HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        if (endpoint == null)
            //如果不是action，默认加载登录信息
            return true;
        var allowAnonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
        if (allowAnonymous != null)
            return false;
        return true;
    }

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

    public static ApiResponse<TokenModel> ToTokenModel(this TokenResponse tokenResponse)
    {
        var res = new ApiResponse<TokenModel>();

        if (tokenResponse.IsError)
        {
            var err = tokenResponse.ErrorDescription ?? tokenResponse.Error ?? "登陆失败";
            return res.SetError(err);
        }

        var model = new TokenModel()
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpireUtc = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
        };

        return res.SetData(model);
    }

    public static async Task<ApiResponse<ClaimsPrincipal>> IsAuthenticatedAsync(this HttpContext context, string scheme)
    {
        if (string.IsNullOrWhiteSpace(scheme))
            throw new ArgumentNullException(nameof(scheme));

        var result = await context.AuthenticateAsync(scheme: scheme);

        if (result.Succeeded && result.Principal.IsAuthenticated(out var claims))
        {
            return new ApiResponse<ClaimsPrincipal>().SetData(result.Principal);
        }
        else
        {
            return new ApiResponse<ClaimsPrincipal>().SetError("error");
        }
    }

    public static bool IsAuthenticated(this HttpContext context, out Claim[] claims)
    {
        claims = null;

        if (context.User.IsAuthenticated(out var c))
        {
            claims = c;
            return true;
        }

        return false;
    }

    public static bool IsAuthenticated(this ClaimsPrincipal principal, out Claim[] claims)
    {
        claims = principal?.Claims?.ToArray() ?? Array.Empty<Claim>();

        var authenticated = principal != null &&
                            principal.Identity != null &&
                            principal.Identity.IsAuthenticated &&
                            claims.Any();

        return authenticated;
    }

    public static string GetSubjectId(this IEnumerable<Claim> claims)
    {
        var res = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
        return res;
    }

    public static ClaimsIdentity SetSubjectId(this ClaimsIdentity identity, string subjectId)
    {
        identity.SetOrReplaceClaim(JwtClaimTypes.Subject, subjectId);
        return identity;
    }

    public static ClaimsIdentity SetIdentityGrantsAll(this ClaimsIdentity identity)
    {
        identity.SetIdentityGrants(new[] { "*" });

        return identity;
    }

    public static ClaimsIdentity SetIdentityGrants(this ClaimsIdentity identity, string[] grants)
    {
        if (ValidateHelper.IsEmptyCollection(grants))
            throw new ArgumentNullException(nameof(grants));

        var grantsData = string.Join(',', grants);
        identity.SetOrReplaceClaim(ClaimTypeConsts.Grants, grantsData);

        return identity;
    }

    public static string[] GetIdentityGrants(this IEnumerable<Claim> claims)
    {
        var res = claims.FirstOrDefault(x => x.Type == ClaimTypeConsts.Grants)?.Value;
        if (string.IsNullOrWhiteSpace(res))
        {
            return Array.Empty<string>();
        }
        else
        {
            var data = res.Split(',').ToArray();
            return data;
        }
    }

    public static ClaimsIdentity SetCreationTime(this ClaimsIdentity identity, DateTime time)
    {
        var timestamp = (int)DateTimeHelper.GetTimeStamp(time);
        identity.SetOrReplaceClaim(ClaimTypeConsts.CreationTime, timestamp.ToString());
        return identity;
    }

    public static DateTime? GetCreationTime(this IEnumerable<Claim> claims)
    {
        var data = claims.FirstOrDefault(x => x.Type == ClaimTypeConsts.CreationTime)?.Value;
        if (!string.IsNullOrWhiteSpace(data) && int.TryParse(data, out var timestamp))
        {
            var res = DateTimeHelper.UTC1970.AddSeconds(timestamp);
            return res;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 先删除，再添加
    /// </summary>
    public static T SetOrReplaceClaim<T>(this T model, string claimType, string data) where T : ClaimsIdentity
    {
        var matched = model.FindAll(x => x.Type == claimType).ToArray();
        foreach (var m in matched)
        {
            model.RemoveClaim(m);
        }

        model.AddClaim(new Claim(claimType, data));
        return model;
    }

    /// <summary>
    /// 获取bearer token
    /// </summary>
    public static string GetBearerToken(this HttpContext context)
    {
        var flag = context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var val);
        if (flag)
        {
            var token = ((string)val).Split(' ').Skip(1).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取这个程序集中所用到的所有权限
    /// </summary>
    public static Dictionary<Type, string[]> ScanAllAssignedPermissionOnThisAssembly(this Assembly ass)
    {
        var data = new Dictionary<Type, string[]>();

        var tps = ass.GetTypes().Where(x => x.IsNormalPublicClass() && x.IsAssignableTo_<ControllerBase>()).ToArray();
        foreach (var t in tps)
        {
            var attrs = t.GetMethods()
                .Where(x => x.IsPublic)
                .Select(x => x.GetCustomAttribute<AdminAuthAttribute>())
                .Where(x => x != null);

            var pers = attrs.SelectMany(x => x.Permission?.Split(',') ?? new string[] { })
                .WhereNotEmpty().ToArray();

            if (ValidateHelper.IsNotEmptyCollection(pers))
            {
                data[t] = pers;
            }
        }

        return data;
    }
}