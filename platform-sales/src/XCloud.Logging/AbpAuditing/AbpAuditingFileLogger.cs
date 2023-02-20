using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using XCloud.Core;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Logging.Serilog;

namespace XCloud.Logging.AbpAuditing;

public class AbpAuditingFileLogger
{
    private readonly IServiceProvider serviceProvider;
    public Microsoft.Extensions.Logging.ILogger Logger { get; }

    void AddLoggerProvider(ILoggingBuilder builder)
    {
        var config = this.serviceProvider.ResolveConfiguration();
        var env = this.serviceProvider.GetRequiredService<IWebHostEnvironment>();

        var savePath = LoggingConfigurationExtension.GetOrCreateLoggingPath(config, env);
        var appName = this.serviceProvider.ResolveAppConfig().AppName();

        var loggerConfig = new LoggerConfiguration().WriteTo.JsonFile(savePath, appName, "auditing", new SerilogOriginMessageFormatter());

        builder.AddProvider(new SerilogLoggerProvider(loggerConfig.CreateLogger()));
    }

    public AbpAuditingFileLogger(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        var logger = LoggerFactory.Create(builder => this.AddLoggerProvider(builder)).CreateLogger<AbpAuditingFileLogger>();

        this.Logger = logger;
    }
}