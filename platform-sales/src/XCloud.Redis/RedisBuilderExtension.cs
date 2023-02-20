using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Builder;
using XCloud.Redis.DistributedLock;

namespace XCloud.Redis;

public static class RedisBuilderExtension
{
    internal static void AddRedisAll(this ServiceConfigurationContext context)
    {
        context.AddRedisClient();
        context.AddRedisDistributedCacheProvider();
        context.AddAbpRedisDistributedCacheProvider();
        
        var builder = context.Services.GetRequiredXCloudBuilder();
        builder.AddRedisDistributedLock();
        builder.AddRedisDataProtectionKeyStore();
    }

    internal static void AddRedisDistributedLock(this IXCloudBuilder builder)
    {
        builder.Services.AddSingleton<RedLockClient>();
    }

    internal static void AddRedisDataProtectionKeyStore(this IXCloudBuilder builder)
    {
        var redisClient = builder.Services.GetRedisClient();

        var appName = AppConfig.GetAppName(builder.Configuration, builder.EntryAssembly);

        builder.Services.RemoveAll<IDataProtectionBuilder>()
            .RemoveAll<IDataProtectionProvider>()
            .RemoveAll<IDataProtector>();

        var dataProtectionBuilder = builder.Services
            .AddDataProtection()
            .SetApplicationName(applicationName: appName)
            .AddKeyManagementOptions(option =>
            {
                option.AutoGenerateKeys = true;
                option.NewKeyLifetime = TimeSpan.FromDays(1000);
            });

        //加密私钥
        var dataProtectionDatabase = (int)RedisConsts.DataProtection;
        dataProtectionBuilder.PersistKeysToStackExchangeRedis(
            databaseFactory: () => redisClient.Connection.SelectDatabase(dataProtectionDatabase),
            key: $"data_protection_key:{appName}");

        builder.SetObject(dataProtectionBuilder);
    }
    
    internal static void AddRedisClient(this ServiceConfigurationContext context)
    {
        var collection = context.Services;
        var config = context.Services.GetConfiguration();

        var connectionString = config.GetRedisConnectionString();

        var connection = new RedisClient(connectionString);
        collection.AddSingleton(connection);
    }

    internal static void AddRedisDistributedCacheProvider(this ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();
        var appName = AppConfig.GetAppName(builder.Configuration, builder.EntryAssembly);

        var redisWrapper = context.Services.GetRedisClient();

        var redisStr = redisWrapper.ConnectionString;

        //remove default memory implementation
        context.Services.RemoveAll<IDistributedCache>();

        context.Services.AddStackExchangeRedisCache(option =>
        {
            option.InstanceName = $"rds-{appName}";

            option.ConfigurationOptions ??= new StackExchange.Redis.ConfigurationOptions();

            option.ConfigurationOptions.EndPoints.Add(redisStr);

            option.ConfigurationOptions.DefaultDatabase = (int)RedisConsts.Caching;
        });
        
        context.Services.Configure<RedisCacheOptions>(options =>
        {
            //
        });
    }

    internal static void AddAbpRedisDistributedCacheProvider(this ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();
        context.Services.Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = $"ids:";
        });
    }
}