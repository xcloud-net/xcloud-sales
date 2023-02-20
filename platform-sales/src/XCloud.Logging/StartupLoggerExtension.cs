using Microsoft.Extensions.Logging;
using Serilog;
using XCloud.Logging.Serilog;

namespace XCloud.Logging;

public static class StartupLoggerExtension
{
    public static ILoggingBuilder AddStartupLoggerProvider(this ILoggingBuilder builder)
    {
        builder.AddSerilog(new LoggerConfiguration().WriteTo.Console().CreateLogger());
        builder.AddSerilog(new LoggerConfiguration().WriteTo.Debug().CreateLogger());
        builder.AddSerilog(new LoggerConfiguration().CreateStartupFileLogger());
        return builder;
    }
}