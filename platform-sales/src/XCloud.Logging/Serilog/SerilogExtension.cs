using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using FluentAssertions;
using XCloud.Core.Helper;

namespace XCloud.Logging.Serilog;

public static class SerilogExtension
{
    public static ILogger CreateStartupFileLogger(this LoggerConfiguration config)
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        new DirectoryInfo(logDir).CreateIfNotExist();

        var logFile = Path.Combine(logDir, "startup.log");

        var logger = config.WriteTo.File(
            path: logFile,
            fileSizeLimitBytes: 1024 * 2014 * 100,
            retainedFileCountLimit: 31,
            rollOnFileSizeLimit: true,
            rollingInterval: RollingInterval.Day).CreateLogger();

        return logger;
    }

    public static LoggerConfiguration JsonFile(this LoggerSinkConfiguration config,
        string savePath, string appName, string logType,
        ITextFormatter textFormatter = null)
    {
        savePath.Should().NotBeNullOrEmpty();
        Directory.Exists(savePath).Should().BeTrue();

        var logPath = Path.Combine(savePath, $"app-dotnet-{appName}-{logType}.log");

        textFormatter ??= new SerilogJsonFormatter();

        var loggerConfig = config.File(
            path: logPath,
            formatter: textFormatter,
            flushToDiskInterval: TimeSpan.FromSeconds(3),
            fileSizeLimitBytes: 1024 * 2014 * 100,
            retainedFileCountLimit: 31,
            rollOnFileSizeLimit: true,
            rollingInterval: RollingInterval.Day);

        return loggerConfig;
    }

    public static string __log_level__(this LogEventLevel level)
    {
        var res = level switch
        {
            LogEventLevel.Debug => nameof(LogEventLevel.Debug),
            LogEventLevel.Error => nameof(LogEventLevel.Error),
            LogEventLevel.Fatal => nameof(LogEventLevel.Fatal),
            LogEventLevel.Information => nameof(LogEventLevel.Information),
            LogEventLevel.Verbose => nameof(LogEventLevel.Verbose),
            LogEventLevel.Warning => nameof(LogEventLevel.Warning),
            _ => Enum.GetName(level) ?? string.Empty,
        };
        return res;
    }
}