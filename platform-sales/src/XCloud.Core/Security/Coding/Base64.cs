namespace XCloud.Core.Security.Coding;

/// <summary>
/// Hash/Base64 encryption.
/// </summary>
public class Base64
{
    public static string EncodeFromStringBytes(byte[] stringBytes)
    {
        if (stringBytes == null)
            throw new ArgumentNullException(nameof(stringBytes));

        var res = Convert.ToBase64String(stringBytes);
        return res;
    }

    public static byte[] DecodeToStringBytes(string base64String)
    {
        if (base64String == null)
            throw new ArgumentNullException(nameof(base64String));

        var bs = Convert.FromBase64String(base64String);

        return bs;
    }
}