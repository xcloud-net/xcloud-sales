using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using XCloud.Core;
using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.Database.EntityFrameworkCore.MySQL;

public static class MySqlExtension
{
    public static void UseMySqlProvider(this DbContextOptionsBuilder option,
        string connectionString,
        uint? poolSize = 10,
        bool includeErrors = true)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ConfigException($"缺少mysql链接字符串配置");

        connectionString = connectionString.GetStructuredMySqlConnectionString(poolSize);

        var version = ServerVersion.AutoDetect(connectionString: connectionString);

        MySqlDbContextOptionsBuilderExtensions.UseMySql(option,
            connectionString: connectionString,
            serverVersion: version,
            mySqlOptionsAction: builder => { builder.CommandTimeout((int)TimeSpan.FromSeconds(10).TotalSeconds); });

        option.EnableDetailedErrors(detailedErrorsEnabled: includeErrors);
        option.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: includeErrors);
    }

    public static void UseMySqlProvider(this DbContextOptionsBuilder option,
        IConfiguration configuration,
        string connectionName,
        uint? poolSize = 10,
        bool includeErrors = true)
    {
        if (string.IsNullOrWhiteSpace(connectionName))
            throw new ArgumentNullException(nameof(connectionName));

        var connectionString = configuration.GetConnectionString(connectionName);

        UseMySqlProvider(option, connectionString: connectionString, poolSize, includeErrors);
    }

    public static void UseMySqlProvider<T>(this AbpDbContextConfigurationContext<T> option,
        string connectionName,
        uint? poolSize = 10,
        bool includeErrors = true) where T : AbpDbContext<T>
    {
        var configuration = option.ServiceProvider.ResolveConfiguration();

        UseMySqlProvider(option.DbContextOptions, configuration, connectionName, poolSize, includeErrors);
    }

    /// <summary>
    /// 格式化mysql链接字符串
    /// </summary>
    public static string GetStructuredMySqlConnectionString(this string cstr, uint? pool_size = 30)
    {
        var builder = new MySqlConnectionStringBuilder(cstr)
        {
            Pooling = false
        };

        if (pool_size != null && pool_size.Value > 0)
        {
            builder.Pooling = true;
            builder.MaximumPoolSize = pool_size.Value;
        }

        cstr = builder.ConnectionString;
        if (string.IsNullOrWhiteSpace(cstr))
            throw new ConfigException("cstr should not be empty");

        return cstr;
    }
}