namespace XCloud.Core.DataSerializer;

public interface IJsonDataSerializer
{
    /// <summary>
    /// 序列化
    /// </summary>
    byte[] SerializeToBytes(object item);
    string SerializeToString(object item);

    /// <summary>
    /// 反序列化
    /// </summary>
    T DeserializeFromBytes<T>(byte[] serializedObject);
    T DeserializeFromString<T>(string serializedObject);
    object DeserializeFromBytes(byte[] serializedObject, Type target);
    object DeserializeFromString(string serializedObject, Type target);
}