using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace XCloud.AspNetMvc.Configuration;

public static class ConfigExtension
{
    private static void TryAddJsonFile(this IConfigurationBuilder builder, string jsonPath)
    {
        if (!File.Exists(jsonPath))
            return;

        builder.AddJsonFile(jsonPath, optional: false);
    }

    private static IConfigurationBuilder TrySettleUpConfig(this IConfigurationBuilder configurationManager,
        string rootPath,
        Action<IConfigurationBuilder> configBuilder = null)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentNullException(nameof(rootPath));

        if (!Directory.Exists(rootPath))
            throw new FileNotFoundException(rootPath);

        var builder = new ConfigurationBuilder();

        builder.TryAddJsonFile(Path.Combine(rootPath, "base.json"));
        builder.TryAddJsonFile(Path.Combine(rootPath, "logging.json"));

        foreach (var m in configurationManager.Sources)
        {
            builder.Sources.Add(m);
        }

        if (configBuilder != null)
        {
            configBuilder.Invoke(builder);
        }

        //finally add placeholder resolver
        builder.AddPlaceholderResolver();
        
        return builder;
    }

    public static void TrySettleUpConfig(this WebApplicationBuilder builder,
        Action<IConfigurationBuilder> configBuilder = null)
    {
        var config = builder.Configuration.TrySettleUpConfig(
                builder.Environment.ContentRootPath,
                configBuilder)
            .Build();

        builder.Services.ReplaceConfiguration(config);
    }
}