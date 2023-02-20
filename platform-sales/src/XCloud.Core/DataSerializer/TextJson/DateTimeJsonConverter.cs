using FluentAssertions;

using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using XCloud.Core.Helper;

namespace XCloud.Core.DataSerializer.TextJson;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private readonly DateTimeFormat format;
    public DateTimeJsonConverter(string DateTimeFormat = null)
    {
        if (!string.IsNullOrWhiteSpace(DateTimeFormat))
        {
            this.DateTimeFormat = DateTimeFormat;
        }
        format = new DateTimeFormat(this.DateTimeFormat);
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(DateTime);

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        {
            var val = reader.GetString();

            if (!string.IsNullOrWhiteSpace(val) && DateTime.TryParse(val, format.FormatProvider, format.DateTimeStyles, out var res))
            {
                return res;
            }
            else
            {
                return default;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            reader.TryGetInt64(out var val).Should().BeTrue();
            var res = DateTimeHelper.UTC1970.AddMilliseconds(val);
            return res;
        }

        throw new NotSupportedException($"{typeToConvert.ToString()}不能转成日期");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var res = value.ToString(DateTimeFormat, format.FormatProvider);
        writer.WriteStringValue(res);
    }
}

public class NullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    private readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private readonly DateTimeFormat format;
    public NullableDateTimeJsonConverter(string DateTimeFormat = null)
    {
        if (!string.IsNullOrWhiteSpace(DateTimeFormat))
        {
            this.DateTimeFormat = DateTimeFormat;
        }
        format = new DateTimeFormat(this.DateTimeFormat);
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(DateTime?);

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        {
            var val = reader.GetString();

            if (!string.IsNullOrWhiteSpace(val) && DateTime.TryParse(val, format.FormatProvider, format.DateTimeStyles, out var res))
            {
                return res;
            }
            else
            {
                return default;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            reader.TryGetInt64(out var val).Should().BeTrue();
            var res = DateTimeHelper.UTC1970.AddMilliseconds(val);
            return res;
        }

        throw new NotSupportedException($"{typeToConvert.ToString()}不能转成日期");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            var res = value.Value.ToString(DateTimeFormat, format.FormatProvider);
            writer.WriteStringValue(res);
        }
    }
}