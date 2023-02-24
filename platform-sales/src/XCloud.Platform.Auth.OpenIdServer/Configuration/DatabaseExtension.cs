using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Modularity;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.Auth.OpenIdServer.Database.EntityFrameworkCore;
using XCloud.Platform.Core;

namespace XCloud.Platform.Auth.OpenIdServer.Configuration;

public static class DatabaseExtension
{
    public static void AddOpenIdDictDbContext(this ServiceConfigurationContext context)
    {
        var services = context.Services;
        var config = context.Services.GetConfiguration();

        services.AddDbContext<OpenIdDictDbContext>(options =>
        {
            options.UseMySqlProvider(config, "openid-dict");
            options.UseOpenIddict();
        });
    }

    public static void TryCreateOpenIdDictDatabase(this IServiceProvider serviceProvider)
    {
        using var s = serviceProvider.CreateScope();
        var logger = s.ServiceProvider.GetRequiredService<ILogger<OpenIdServerModule>>();
        if (!s.ServiceProvider.AutoCreatePlatformDatabase())
        {
            logger.LogInformation("skip create openid dict database");
            return;
        }

        using var db = s.ServiceProvider.GetRequiredService<OpenIdDictDbContext>();
        db.Database.EnsureCreated();
    }
}