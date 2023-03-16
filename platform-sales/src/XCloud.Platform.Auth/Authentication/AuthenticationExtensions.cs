using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Data;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Auth.Application.Admin.Filter;

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

    public static ParsedTokenDto GetParsedTokenDto(this IEnumerable<Claim> claims)
    {
        var claimsArr = claims.ToArray();
        
        var token = new ParsedTokenDto()
        {
            Id = claimsArr.GetSubjectId(),
            CreationTime = claimsArr.GetCreationTime() ?? DateTimeHelper.UTC1970
        };

        foreach (var m in claimsArr)
        {
            token.SetProperty(m.Type, m.Value);
        }

        return token;
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
        identity.SetOrReplaceClaim(PlatformClaimTypes.Grants, grantsData);

        return identity;
    }

    public static string[] GetIdentityGrants(this IEnumerable<Claim> claims)
    {
        var res = claims.FirstOrDefault(x => x.Type == PlatformClaimTypes.Grants)?.Value;
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
        identity.SetOrReplaceClaim(PlatformClaimTypes.CreationTime, timestamp.ToString());
        return identity;
    }

    public static DateTime? GetCreationTime(this IEnumerable<Claim> claims)
    {
        var data = claims.FirstOrDefault(x => x.Type == PlatformClaimTypes.CreationTime)?.Value;
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
        var hasTokenHeader = context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var val);

        if (hasTokenHeader)
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