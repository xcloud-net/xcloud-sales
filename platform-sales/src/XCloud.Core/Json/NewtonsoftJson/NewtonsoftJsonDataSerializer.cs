using System.Text;
using Newtonsoft.Json;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Configuration;
using XCloud.Core.Helper;

namespace XCloud.Core.Json.NewtonsoftJson;

public class NewtonsoftJsonDataSerializer : IJsonDataSerializer
{
    private readonly Encoding _encoding;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public NewtonsoftJsonDataSerializer(
        AppConfig appconfig,
        INewtonsoftJsonOptionAccessor newtonsoftJsonOptionAccesser)
    {
        this._encoding = appconfig.Encoding;
        this._jsonSerializerSettings = newtonsoftJsonOptionAccesser.SerializerSettings;
    }

    public virtual byte[] SerializeToBytes(object item)
    {
        try
        {
            var jsonString = this.SerializeToString(item);

            var res = this._encoding.GetBytes(jsonString);

            return res;
        }
        catch (System.Exception e)
        {
            throw new SerializeException(nameof(SerializeToBytes), e);
        }
    }

    public virtual T DeserializeFromBytes<T>(byte[] serializedObject)
    {
        try
        {
            if (ValidateHelper.IsEmptyCollection(serializedObject))
                throw new ArgumentNullException(nameof(serializedObject));

            var json = this._encoding.GetString(serializedObject);

            var res = this.DeserializeFromString<T>(json);

            return res;
        }
        catch (System.Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromBytes), e);
        }
    }

    public object DeserializeFromBytes(byte[] serializedObject, Type target)
    {
        try
        {
            if (ValidateHelper.IsEmptyCollection(serializedObject))
                throw new ArgumentNullException(nameof(serializedObject));

            var json = this._encoding.GetString(serializedObject);

            var obj = this.DeserializeFromString(json, target);

            return obj;
        }
        catch (System.Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromBytes), e);
        }
    }

    public string SerializeToString(object obj)
    {
        try
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var res = JsonConvert.SerializeObject(obj,
                settings: this._jsonSerializerSettings);
            return res;
        }
        catch (System.Exception e)
        {
            throw new SerializeException(nameof(SerializeToString), e);
        }
    }

    public T DeserializeFromString<T>(string serializedObject)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(serializedObject))
                throw new ArgumentNullException(nameof(serializedObject));

            var res = JsonConvert.DeserializeObject<T>(serializedObject,
                settings: this._jsonSerializerSettings);
            return res;
        }
        catch (System.Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromString), e);
        }
    }

    public object DeserializeFromString(string serializedObject, Type target)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(serializedObject))
                throw new ArgumentNullException(nameof(serializedObject));

            var res = JsonConvert.DeserializeObject(serializedObject, type: target,
                settings: this._jsonSerializerSettings);
            return res;
        }
        catch (System.Exception e)
        {
            throw new SerializeException(nameof(DeserializeFromString), e);
        }
    }
}