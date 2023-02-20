namespace XCloud.Core.Extension;

public static class LoggerExtension
{
    public static ILoggerFactory ResolveLoggerFactory(this IServiceProvider provider)
    {
        var res = provider.GetRequiredService<ILoggerFactory>();
        return res;
    }

    public static ILogger<T> ResolveLogger<T>(this IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<T>>();
        return logger;
    }

    public static void AddErrorLog(this ILogger logger, string msg, Exception e = null)
    {
        var message = msg ?? e?.Message ?? string.Empty;

        if (e != null)
        {
            logger.LogError(message: message, exception: e);
        }
        else
        {
            logger.LogError(message: message);
        }
    }

    public static string LogLevelAsString(this LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Critical:
                return nameof(LogLevel.Critical);
            case LogLevel.Debug:
                return nameof(LogLevel.Debug);
            case LogLevel.Error:
                return nameof(LogLevel.Error);
            case LogLevel.Information:
                return nameof(LogLevel.Information);
            case LogLevel.None:
                return nameof(LogLevel.None);
            case LogLevel.Trace:
                return nameof(LogLevel.Trace);
            case LogLevel.Warning:
                return nameof(LogLevel.Warning);
            default:
                return string.Empty;
        }
    }
}