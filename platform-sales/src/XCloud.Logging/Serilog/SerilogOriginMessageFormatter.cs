using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace XCloud.Logging.Serilog;

public class SerilogOriginMessageFormatter : ITextFormatter
{
    public SerilogOriginMessageFormatter()
    {
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        if (logEvent == null || output == null)
        {
            return;
        }

        var message = logEvent.RenderMessage();

        output.Write(message);
        output.WriteLine();
    }
}