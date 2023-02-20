using System.Text;
using System.Text.Json;
using XCloud.Core.Application.WorkContext;

namespace XCloud.Core.DataSerializer.TextJson;

public class TextJsonDataSerializer : IJsonDataSerializer
{
    private readonly Encoding _encoding;
    private readonly JsonSerializerOptions _jsonSerializerSettings;

    public TextJsonDataSerializer(
        AppConfig appconfig,
        ITextJsonOptionAccessor newtonsoftJsonOptionAccesser)
    {
        this._encoding = appconfig.Encoding;
        this._jsonSerializerSettings = newtonsoftJsonOptionAccesser.SerializerSettings;
    }

    public T DeserializeFromBytes<T>(byte[] serializedObject)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<T>(serializedObject, this._jsonSerializerSettings);

            return obj;
        }
        catch (Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromBytes), e);
        }
    }

    public object DeserializeFromBytes(byte[] serializedObject, Type target)
    {
        try
        {
            var obj = JsonSerializer.Deserialize(serializedObject, target, this._jsonSerializerSettings);

            return obj;
        }
        catch (Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromBytes), e);
        }
    }

    public T DeserializeFromString<T>(string serializedObject)
    {
        try
        {
            var bs = this._encoding.GetBytes(serializedObject);

            return this.DeserializeFromBytes<T>(bs);
        }
        catch (Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromString), e);
        }
    }

    public object DeserializeFromString(string serializedObject, Type target)
    {
        try
        {
            var bs = this._encoding.GetBytes(serializedObject);

            return this.DeserializeFromBytes(bs, target);
        }
        catch (Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromString), e);
        }
    }

    public byte[] SerializeToBytes(object item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        try
        {
            var bs = JsonSerializer.SerializeToUtf8Bytes(item, item.GetType(), this._jsonSerializerSettings);

            return bs;
        }
        catch (Exception e)
        {
            throw new SerializeException(nameof(SerializeToBytes), e);
        }
    }

    public string SerializeToString(object item)
    {
        try
        {
            var bs = this.SerializeToBytes(item);

            var jsonString = this._encoding.GetString(bs);

            return jsonString;
        }
        catch (Exception e)
        {
            throw new SerializeException(nameof(SerializeToString), e);
        }
    }
}