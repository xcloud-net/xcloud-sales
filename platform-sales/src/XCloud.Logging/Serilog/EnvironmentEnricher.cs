using Serilog.Core;
using Serilog.Events;
using System;

namespace XCloud.Logging.Serilog;

internal class EnvironmentEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var os = Environment.OSVersion;
        if (os != null)
        {
            var platformName = Enum.GetName<PlatformID>(os.Platform) ?? "unknown";
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("platform_name", platformName));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("platform_version", os.VersionString));
        }
    }
}