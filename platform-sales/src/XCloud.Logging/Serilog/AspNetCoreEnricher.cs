using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System.Threading;

namespace XCloud.Logging.Serilog;

internal class AspNetCoreEnricher : ILogEventEnricher
{
    public static readonly AsyncLocal<IHttpContextAccessor> HttpContextAccessor = new AsyncLocal<IHttpContextAccessor>();

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = HttpContextAccessor.Value?.HttpContext;
        if (httpContext != null)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("request_url", httpContext.Request.Path));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("http_method", httpContext.Request.Method));

            if (httpContext.Request.Headers.TryGetValue("client_name", out var name))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("client_name", name));
            }
        }
    }
}