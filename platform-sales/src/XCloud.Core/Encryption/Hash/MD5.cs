using FluentAssertions;
using System.Text;

namespace XCloud.Core.Encryption.Hash;

/// <summary>
/// MD5 encryption.
/// </summary>
public class MD5
{
    public static byte[] EncryptBytes(byte[] bs)
    {
        bs.Should().NotBeNull();
        using var md5 = System.Security.Cryptography.MD5.Create();

        var res = md5.ComputeHash(bs);

        return res;
    }

    public static string Encrypt(byte[] data)
    {
        var bs = EncryptBytes(data);

        var result = EncryptionUtils.BsToStr(bs);

        return result;
    }

    /// <summary>
    /// MD5 encrypt with 32 bits.
    /// </summary>
    /// <param name="str">The string of encrypt.</param>
    /// <param name="encoding">The <see cref="T:System.Text.Encoding"/>,default is Encoding.UTF8.</param>
    /// <returns>Encrypt string.</returns>
    public static string Encrypt(string str, Encoding encoding)
    {
        str.Should().NotBeNull();
        encoding.Should().NotBeNull();

        var data = encoding.GetBytes(str);

        var res = Encrypt(data);

        return res;
    }
}