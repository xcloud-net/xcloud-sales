using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Volo.Abp.Auditing;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Core.Configuration;
using XCloud.Core.Configuration.Builder;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Logging.AbpAuditing;
using XCloud.Logging.Serilog;

namespace XCloud.Logging;

public static class LoggingConfigurationExtension
{
    internal static void ConfigAuditing(this ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();
        var config = context.Services.GetConfiguration();
        var isAuditingEnabled = (config["app:config:audit"] ?? "false").ToBool();

        //审计日志配置
        context.Services.Configure<AbpAuditingOptions>(option =>
        {
            option.IsEnabled = isAuditingEnabled;
            option.IsEnabledForGetRequests = false;
            option.HideErrors = false;
            option.ApplicationName = AppConfig.GetAppName(config, builder.EntryAssembly);
        });
        context.Services.AddSingleton<AbpAuditingFileLogger>();
        context.Services.RemoveAll<IAuditingStore>().AddTransient<IAuditingStore, AbpAuditingFileAuditingStore>();
    }

    internal static void AddLoggingAll(this ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();
        var env = context.Services.GetSingletonInstanceOrNull<IWebHostEnvironment>();
        var loggingDir = LoggingConfigurationExtension.GetOrCreateLoggingPath(config, env);

        var builder = context.Services.GetRequiredXCloudBuilder();
        var appName = AppConfig.GetAppName(config, builder.EntryAssembly);

        context.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();

#if DEBUG
            //生产环境过滤掉trace
            var productionLogLevel = new[]
            {
                LogLevel.Information,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Critical
            };
            loggingBuilder.AddFilter(level => productionLogLevel.Contains(level));

            //console debug
            //loggingBuilder.AddConsole();
            //loggingBuilder.AddDebug();
#endif

            loggingBuilder.AddSerilogProvider(config, appName, loggingDir);
        });
    }
    
    public static bool IsDebug(this IConfiguration config) => config["app:config:debug"]?.ToLower() == "true";

    public static string GetLogDir(this IConfiguration config) => config["app:config:log_base_dir"];

    public static string GetOrCreateLoggingPath(IConfiguration config, IWebHostEnvironment envOrNull)
    {
        var configuredLoggingDir = config.GetLogDir();
        
        if (string.IsNullOrWhiteSpace(configuredLoggingDir))
        {
            if (envOrNull != null)
            {
                configuredLoggingDir = Path.Combine(envOrNull.ContentRootPath, "logs");
            }
            else
            {
                configuredLoggingDir = Directory.GetCurrentDirectory();
            }
        }

        if (string.IsNullOrWhiteSpace(configuredLoggingDir))
            throw new ConfigException($"{nameof(configuredLoggingDir)} is required");
        
        new DirectoryInfo(configuredLoggingDir).CreateIfNotExist();
        
        return configuredLoggingDir;
    }

    private static void AddSerilogProvider(this ILoggingBuilder builder, IConfiguration config, string appName, string logPath)
    {
        LoggerConfiguration LoggerConfig() => new LoggerConfiguration()
            //https://github.com/serilog/serilog-settings-configuration
            .ReadFrom.Configuration(config, sectionName: "Serilog")
            .Enrich.WithProperty("app_name", appName);

        var hostEnv = builder.Services.GetHostingEnvironment();

        if (hostEnv.IsDevelopment() || hostEnv.IsStaging() || config.IsDebug())
        {
            builder.AddSerilog(LoggerConfig().WriteTo.Console().CreateLogger());
            builder.AddSerilog(LoggerConfig().WriteTo.Debug().CreateLogger());
        }

        //https://github.com/serilog/serilog-sinks-file

        var infoLogger = LoggerConfig()
            .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Information)
            .WriteTo.JsonFile(logPath, appName, "info")
            .CreateLogger();
        builder.AddSerilog(infoLogger);

        var warningLogger = LoggerConfig()
            .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Warning)
            .WriteTo.JsonFile(logPath, appName, "warning")
            .CreateLogger();
        builder.AddSerilog(warningLogger);

        var errorLevelType = new[] { LogEventLevel.Error, LogEventLevel.Fatal };
        var errorLogger = LoggerConfig()
            .Enrich.With(new DemystifiedStackTraceEnricher())
            .Enrich.With(new AssemblyEnricher())
            .Enrich.With(new EnvironmentEnricher())
            .Enrich.With(new AspNetCoreEnricher())
            .Filter.ByIncludingOnly(x => errorLevelType.Contains(x.Level))
            .WriteTo.JsonFile(logPath, appName, "error-fatal")
            .CreateLogger();
        builder.AddSerilog(errorLogger);

        //sentry-optional provider
        var sentryDsn = config["Sentry:Dsn"]?.Trim();
        if (!string.IsNullOrWhiteSpace(sentryDsn))
        {
            var sentryLogger = LoggerConfig()
                .Filter.ByIncludingOnly(x => errorLevelType.Contains(x.Level))
                .WriteTo.Sentry(option =>
                {
                    option.Dsn = sentryDsn;
                    option.MinimumEventLevel = global::Serilog.Events.LogEventLevel.Error;
                }).CreateLogger();
            builder.AddSerilog(sentryLogger);
        }
        //if you are not assigning the logger to Log.Logger, then you need to add your logger here.
    }
}