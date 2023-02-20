using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using XCloud.Platform.Shared;
using System.Text;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Auth.Authentication;

namespace XCloud.Platform.Auth;

public static class AuthConfigurationExtension
{
    /// <summary>
    /// 使用identity server校验token
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static void AddIdentityServerBearerAuthentication(this AuthenticationBuilder builder)
    {
        var config = builder.Services.GetConfiguration();

        var scheme = IdentityConsts.Scheme.BearerTokenScheme;
        var identityServer = config.GetIdentityServerAddressOrThrow();

        builder.AddIdentityServerAuthentication(scheme, option =>
        {
            option.SupportedTokens = SupportedTokens.Both;
            option.Authority = identityServer;
            option.ApiName = "water";
            //只有使用reference token才需要配置api secret
            option.ApiSecret = "456";
            option.RequireHttpsMetadata = false;
        });
    }

    private static void AddJwtAuthentication(this AuthenticationBuilder builder)
    {
        var scheme = IdentityConsts.Scheme.JwtTokenScheme;

        builder.AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true, //是否验证Issuer
                ValidateAudience = true, //是否验证Audience
                ValidateLifetime = true, //是否验证失效时间
                ClockSkew = TimeSpan.FromSeconds(30),
                ValidateIssuerSigningKey = true, //是否验证SecurityKey
                ValidAudience = "http://localhost:5000", //Audience
                ValidIssuer = "http://localhost:5000", //Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456789123456789123456789")) //拿到SecurityKey
            };
        });
    }

    /// <summary>
    /// 基于cookie的登陆验证
    /// </summary>
    private static void AddCookieAuthentication(this AuthenticationBuilder builder)
    {
        var cookieScheme = IdentityConsts.Scheme.IdentityServerWebCookieScheme;

        builder.AddCookie(cookieScheme, option =>
        {
            //option.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;

            option.Cookie.Name = $"ids_web_login_{cookieScheme}";
            option.ExpireTimeSpan = TimeSpan.FromDays(30);
            option.LoginPath = "/Account/Login";
            option.LogoutPath = "/Account/Logout";
        });
    }

    private static void AddExternalCookieAuthentication(this AuthenticationBuilder builder)
    {
        var cookieExternalScheme = IdentityConsts.Scheme.ExternalLoginScheme;

        builder.AddCookie(cookieExternalScheme, option =>
        {
            //option.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;

            option.Cookie.Name = $"ids_web_external_login_{cookieExternalScheme}";
            //三方登陆的信息只保存10分钟，请尽快引导用户登陆自己账号体系的账号
            option.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        });
    }

    public static void AddUserAuthentication(this IServiceCollection collection)
    {
        //认证配置
        var defaultAuthScheme = IdentityConsts.Scheme.BearerTokenScheme;

        var builder = collection.AddAuthentication(defaultScheme: defaultAuthScheme);
        
        builder.AddCookieAuthentication();
        builder.AddExternalCookieAuthentication();
        builder.AddIdentityServerBearerAuthentication();
        //break identity server bearer token validation
        //builder.AddJwtAuthentication();

        collection.Configure<AuthenticationOptions>(option => { option.DefaultScheme = defaultAuthScheme; });
    }

    /// <summary>
    /// 权限验证
    /// </summary>
    public static void AddAuthorizationHandlers([NotNull] this IServiceCollection serviceCollection)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
        //collection.RemoveAll<IAuthorizationHandler>();
        serviceCollection.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();
        //serviceCollection.AddSingleton<IAuthorizationHandler, AdminDepartmentAuthorizationHandler>();
    }
}