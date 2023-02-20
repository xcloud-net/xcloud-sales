using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace XCloud.Logging.Serilog;

internal class AssemblyEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        if (assembly != null)
        {
            var assemblyName = assembly.GetName();
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("AssemblyName", assemblyName.FullName));

            var version = assemblyName.Version;
            if (version != null)
            {
                string versionStr = $"{version.Major}.{version.Minor}.{version.Build}";
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("AssemblyVersion", versionStr));
            }
        }
    }
}