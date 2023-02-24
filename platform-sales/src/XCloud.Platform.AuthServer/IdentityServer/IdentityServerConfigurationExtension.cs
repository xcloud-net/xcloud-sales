using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc.Builder;
using XCloud.Core;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Platform.AuthServer.IdentityServer.GrantValidator;
using XCloud.Platform.AuthServer.IdentityServer.PersistentStore;
using XCloud.Platform.Shared;

namespace XCloud.Platform.AuthServer.IdentityServer;

public static class IdentityServerConfigurationExtension
{
    /// <summary>
    /// 配置identity server
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddIdentityServerComponents(this IServiceCollection collection)
    {
        var config = collection.GetConfiguration();
        var env = collection.GetHostingEnvironment();

        var identityBuilder = collection.AddIdentityServer(option =>
        {
            //option.UserInteraction.ErrorUrl = "";
            /*
             * 升级到v4后这个选项不见了，改为在中间件中设置publicorigin
             var publicOrigin = config["public_origin"];
            if (publicOrigin?.Length > 0)
            {
                option.PublicOrigin = publicOrigin;
            }
             */
            option.Authentication ??= new IdentityServer4.Configuration.AuthenticationOptions();
            option.Authentication.CookieAuthenticationScheme = 
                IdentityConsts.Scheme.IdentityServerWebCookieScheme;
                
            option.Endpoints.EnableJwtRequestUri = true;
        });

        //identityBuilder.AddDeveloperSigningCredential();
        var pfx = Path.Combine(env.ContentRootPath, "idsrv4.pfx");
        File.Exists(pfx).Should().BeTrue($"ids证书不存在:{pfx}");

        identityBuilder.AddSigningCredential(new X509Certificate2(pfx, "1234"));

        //微信授权登陆
        identityBuilder.AddResourceOwnerValidator<UserPasswordValidator>();
        identityBuilder.AddExtensionGrantValidator<InternalGrantValidator>();
        //身份信息
        identityBuilder.AddProfileService<PlatformProfileService>();
        //允许的跳转地址
        identityBuilder.AddRedirectUriValidator<PlatformRedirectUriValidator>();

        //identityBuilder.AddClientStoreCache();
        identityBuilder.AddInMemoryCaching();
        //使用通用的idistributedcache
        identityBuilder.AddConfigurationStoreCache();

        return identityBuilder;
    }

    /// <summary>
    /// ids 4版本后取消了option中的public origin配置，于是再请求管道中设置这个值
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MvcPipelineBuilder SetIdentityPublicOrigin(this MvcPipelineBuilder builder)
    {
        builder.App.Use(async (context, next) =>
        {
            //升级到v4后这public origin选项不见了，改为在中间件中设置public origin
            var config = context.RequestServices.ResolveConfiguration();
            var publicOrigin = config["public_origin"];
            if (publicOrigin?.Length > 0)
            {
                context.SetIdentityServerOrigin(publicOrigin);
                context.SetIdentityServerBasePath(context.Request.PathBase.Value?.TrimEnd('/'));
            }
            await next.Invoke();
        });
        return builder;
    }

    public static bool IntegrateIdentityServer(this IConfiguration configuration)
    {
        return configuration["app:identity_server:IntegrateIdentityServer"] == "true";
    }

    public static void ConfigAuthServer(this ServiceConfigurationContext context)
    {
        //identity server
        var config = context.Services.GetConfiguration();
        if (IntegrateIdentityServer(config))
        {
            //ids配置
            context.Services
                .AddIdentityServerComponents()
                .AddOperationStore()
                .AddConfigurationStore();
        }
    }
}