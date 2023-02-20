namespace XCloud.Core.Encryption;

public static class EncryptionUtils
{
    public static string BsToStr(byte[] bs)
    {
        var arr = bs.Select(x => x.ToString("x2")).ToArray();
        var data = string.Join(string.Empty, arr);
        data = data.Replace("-", string.Empty);
        data = data.ToLower();
        return data;
    }
}