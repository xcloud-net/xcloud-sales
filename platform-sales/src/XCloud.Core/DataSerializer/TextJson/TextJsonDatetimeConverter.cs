using System.Text.Json;
using System.Text.Json.Serialization;

namespace XCloud.Core.DataSerializer.TextJson;

public class TextJsonDatetimeConverter : JsonConverter<DateTime>
{
    private readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var strValue = reader.GetString();
        if (!string.IsNullOrWhiteSpace(strValue) && DateTime.TryParse(strValue, out var d))
        {
            return d;
        }
        throw new NotSupportedException($"无法把'{strValue}'转成datetime格式");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var strValue = value.ToString(format: DateTimeFormat);
        writer.WriteStringValue(strValue);
    }
}