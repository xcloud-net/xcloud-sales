using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using XCloud.Platform.Auth.OpenIdServer.Database.EntityFrameworkCore;

namespace XCloud.Platform.Auth.OpenIdServer.Configuration;

public static class OpenIdDictExtension
{
    public static void AddOpenIdDictServer(this ServiceConfigurationContext context)
    {
        context.Services.AddOpenIddict()
            .AddCore(options => { options.UseEntityFrameworkCore().UseDbContext<OpenIdDictDbContext>(); })
            .AddServer(options =>
            {
                options
                    .SetAuthorizationEndpointUris("/connect/authorize")
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
}