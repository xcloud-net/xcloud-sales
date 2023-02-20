using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using System.Linq;
using XCloud.Core.Extension;

namespace XCloud.Logging.Serilog;

public class SerilogJsonFormatter : ITextFormatter
{
    //private readonly CultureInfo cultureInfo = new CultureInfo("");
    public SerilogJsonFormatter() { }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        if (logEvent == null || output == null)
        {
            return;
        }

        var all_properties = logEvent.Properties.ToDict(x => x.Key, x => LogEventPropertyValueVisitor.Visit(x.Value));
        //https://github.com/serilog/serilog/issues/91#issuecomment-37532568
        var logger_name = all_properties
            .Where(x => string.Equals(x.Key, "SourceContext", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value).FirstOrDefault();

        var data = new
        {
            logger = logger_name,
            timestamp = logEvent.Timestamp,
            level = logEvent.Level.__log_level__().ToUpper(),
            message = logEvent.RenderMessage(),
            exception = logEvent.Exception?.ExtractExceptionDescriptor(8),
            properties = all_properties,
        };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        output.Write(json);
        output.WriteLine();
    }
}