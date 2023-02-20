using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.Core.Builder;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.Framework;

namespace XCloud.Platform.OpenIdServer;

public class OpenIdDictDbContext : DbContext
{
    public OpenIdDictDbContext(DbContextOptions<OpenIdDictDbContext> options) : base(options)
    {
        //
    }
}

[DependsOn(
    typeof(PlatformFrameworkModule)
)]
public class OpenIdServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddXCloudBuilder<OpenIdServerModule>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var config = context.Services.GetConfiguration();

        services.AddDbContext<OpenIdDictDbContext>(options =>
        {
            options.UseMySqlProvider(config, "openid-dict");
            options.UseOpenIddict();
        });

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<OpenIdDictDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                    .SetLogoutEndpointUris("/connect/logout")
                    .SetTokenEndpointUris("/connect/token")
                    .SetUserinfoEndpointUris("/connect/userinfo");

                options.AllowAuthorizationCodeFlow();
                options.AllowClientCredentialsFlow();
                //options.AllowDeviceCodeFlow();
                options.AllowHybridFlow();
                options.AllowImplicitFlow();
                //options.AllowNoneFlow();
                options.AllowPasswordFlow();
                options.AllowRefreshTokenFlow();
                //options.AllowCustomFlow();
                options.SetAccessTokenLifetime(TimeSpan.FromDays(100));
                options.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(3));

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core options.
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .DisableTransportSecurityRequirement();

            })
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();
                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAuditing();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });

        app.UseWelcomePage();
    }
}