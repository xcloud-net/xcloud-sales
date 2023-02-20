using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace XCloud.Logging.Serilog;

internal class DemystifiedStackTraceEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception != null)
        {
            logEvent.Exception.Demystify();
        }
    }
}