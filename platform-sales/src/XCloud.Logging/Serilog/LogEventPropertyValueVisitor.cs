using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace XCloud.Logging.Serilog;

public static class LogEventPropertyValueVisitor
{
    /// <summary>
    /// 自己实现的，不知道serilog有没有现成提供的
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static object Visit(LogEventPropertyValue input)
    {
        var visited = new List<LogEventPropertyValue>();
        object __visit__(LogEventPropertyValue value)
        {
            if (value == null || visited.Contains(value))
            {
                return null;
            }
            visited.Add(value);
            //Serilog.Events.LogEventPropertyValue
            if (value is ScalarValue s)
            {
                return s.Value;
            }

            if (value is DictionaryValue d)
            {
                var res = d.Elements.Select(x => new { Key = x.Key.Value, Value = __visit__(x.Value) }).ToArray();
                return res;
            }

            if (value is SequenceValue seq)
            {
                var res = seq.Elements.Select(x => __visit__(x)).ToArray();
                return res;
            }

            if (value is StructureValue stru)
            {
                var res = stru.Properties.Select(x => new { x.Name, Value = __visit__(x.Value) }).ToArray();

                return new
                {
                    stru.TypeTag,
                    structure_data = res
                };
            }

            return value;
        }

        return __visit__(input);
    }
}