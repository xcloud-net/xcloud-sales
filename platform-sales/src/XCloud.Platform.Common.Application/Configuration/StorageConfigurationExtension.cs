using System.IO;
using Microsoft.Extensions.Configuration;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Platform.Common.Application.Service.Storage.AbpFileSystem;
using XCloud.Platform.Common.Application.Service.Storage.QCloud;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Common.Application.Configuration;

public static class StorageConfigurationExtension
{
    internal static void ConfigStorageAll(this ServiceConfigurationContext context)
    {
        context.ConfigDefaultStorage();
        context.ConfigFileStorage();
        context.TryConfigQCloudStorage();

        context.Services.RemoveAll<IBlobFilePathCalculator>();
        context.Services.AddSingleton<IBlobFilePathCalculator, BlobFileKeyShardingPathCalculator>();
    }

    private static string GetOrCreateSavePath(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var savePath = configuration["app:config:static_save_path"];

        if (string.IsNullOrWhiteSpace(savePath))
        {
            var env = context.Services.GetHostingEnvironment();
            savePath = Path.Combine(env.ContentRootPath, "static_files");
        }

        new DirectoryInfo(savePath).CreateIfNotExist();

        return savePath;
    }

    private static IConfigurationSection GetQCloudSection(this IConfiguration configuration)
    {
        var tencentCloudSection = configuration.GetSection("TencentCloud");

        return tencentCloudSection;
    }

    private static void UseQCloudStorageProvider(this BlobContainerConfiguration blobContainerConfiguration,
        IConfiguration configuration)
    {
        //bind configuration
        var tencentCloudSection = configuration.GetQCloudSection();
        if (!tencentCloudSection.Exists())
        {
            throw new ConfigException("qcloud storage config not found");
        }

        tencentCloudSection.Bind(new TencentCloudBlobProviderConfiguration(blobContainerConfiguration));

        //set target provider type
        blobContainerConfiguration.ProviderType = typeof(TencentCloudBlobProvider);
    }

    public static string GetDefaultStorageProvider(this IConfiguration configuration)
    {
        var storageProvider = configuration["app:config:StorageProvider"];

        if (string.IsNullOrWhiteSpace(storageProvider))
            return StorageProviders.AbpFsBlobProvider1;

        return storageProvider;
    }

    public static bool IsThumborEnabled(this IConfiguration configuration)
    {
        var enabled = (configuration["app:config:ThumborEnabled"] ?? "false").ToBool();
        return enabled;
    }

    private static void ConfigDefaultStorage(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var savePath = GetOrCreateSavePath(context);
        var providerName = configuration.GetDefaultStorageProvider();

        context.Services.Configure<AbpBlobStoringOptions>(option =>
        {
            //config default storage provider
            option.Containers.ConfigureDefault(config =>
            {
                if (providerName == StorageProviders.QCloudBlobProvider1)
                {
                    config.UseQCloudStorageProvider(configuration);
                }
                else if (providerName == StorageProviders.AbpFsBlobProvider1)
                {
                    config.UseFileSystem(f =>
                    {
                        f.AppendContainerNameToBasePath = true;
                        f.BasePath = savePath;
                    });
                }
                else
                {
                    throw new ConfigException("unsupported storage provider name");
                }
            });
        });
    }

    private static void ConfigFileStorage(this ServiceConfigurationContext context)
    {
        var savePath = GetOrCreateSavePath(context);
        context.Services.Configure<AbpBlobStoringOptions>(option =>
        {
            option.Containers.Configure(StorageProviders.AbpFsBlobProvider1, config =>
            {
                config.UseFileSystem(f =>
                {
                    f.AppendContainerNameToBasePath = true;
                    f.BasePath = savePath;
                });
            });
        });
    }

    private static void TryConfigQCloudStorage(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var section = configuration.GetQCloudSection();

        if (!section.Exists())
            return;

        context.Services.Configure<AbpBlobStoringOptions>(option =>
        {
            option.Containers.Configure(StorageProviders.QCloudBlobProvider1,
                containerConfiguration => { containerConfiguration.UseQCloudStorageProvider(configuration); });
        });
    }
}