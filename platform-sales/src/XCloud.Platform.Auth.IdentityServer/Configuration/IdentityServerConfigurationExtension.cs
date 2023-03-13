using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc.Builder;
using XCloud.Core;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Platform.Auth.IdentityServer.Database;
using XCloud.Platform.Auth.IdentityServer.Provider;
using XCloud.Platform.Shared.Constants;

namespace XCloud.Platform.Auth.IdentityServer.Configuration;

public static class IdentityServerConfigurationExtension
{
    /// <summary>
    /// 配置identity server
    /// </summary>
    private static IIdentityServerBuilder AddIdentityServerComponents(this IServiceCollection collection)
    {
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
        if (!File.Exists(pfx))
            throw new ConfigException($"ids证书不存在:{pfx}");

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
    public static void SetIdentityPublicOrigin(this MvcPipelineBuilder builder)
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
    }

    public static void ConfigIdentityServer(this ServiceConfigurationContext context)
    {
        //identity server
        context.Services
            .AddIdentityServerComponents()
            .AddOperationStore()
            .AddConfigurationStore();
    }
}