using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FluentAssertions;

namespace XCloud.Core.Security.Asymmetric;

/// <summary>
/// Asymmetric/RSA encryption.
/// </summary>
public class RSA
{
    /// <summary>
    /// Get public and private key of xml format.
    /// </summary>
    /// <param name="xmlPrivateKey">The private key of xml format.</param>
    /// <param name="xmlPublicKey">The public key of xml format.</param>
    /// <param name="dwKeySize">The size of the key to use in bits</param>
    public static void CreateKey(out string xmlPrivateKey, out string xmlPublicKey, int dwKeySize = 1024)
    {
        using var rsa = new RSACryptoServiceProvider(dwKeySize);
        xmlPrivateKey = rsa.ToXmlString(true);
        xmlPublicKey = rsa.ToXmlString(false);
    }


    /// <summary>
    /// Get private key of xml format from certificate file.
    /// </summary>
    /// <param name="certFile">The string path of certificate file.</param>
    /// <param name="password">The string password of certificate file.</param>
    /// <returns>String private key of xml format.</returns>
    public static string GetPrivateKey(string certFile, string password)
    {
        certFile.Should().NotBeNullOrEmpty();
        password.Should().NotBeNullOrEmpty();
        File.Exists(certFile).Should().BeTrue();

        using var cert = new X509Certificate2(certFile, password, X509KeyStorageFlags.Exportable);
        string privateKey = cert.GetRSAPrivateKey()?.ToXmlString(true);
        return privateKey;
    }

    /// <summary>
    /// Get public key of xml format from certificate file.
    /// </summary>
    /// <param name="certFile">The string path of certificate file.</param>
    /// <returns>String public key of xml format.</returns>
    public static string GetPublicKey(string certFile)
    {
        certFile.Should().NotBeNullOrEmpty();
        File.Exists(certFile).Should().BeTrue();

        using var cert = new X509Certificate2(certFile);
        string publicKey = cert.GetRSAPublicKey()?.ToXmlString(false);
        return publicKey;
    }

    #region encrypt/decrypt string or byte[] with xml format.

    public static byte[] EncryptBytes(byte[] dataBytes, string xmlPublicKey)
    {
        dataBytes.Should().NotBeNull();
        xmlPublicKey.Should().NotBeNullOrEmpty();

        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(xmlPublicKey);
            byte[] bytes = rsa.Encrypt(dataBytes, false);
            return bytes;
        }
    }

    /// <summary>
    /// Encrypt byte[] data with xml format.
    /// </summary>
    /// <param name="dataBytes">The data to be encrypted.</param>
    /// <param name="xmlPublicKey">The public key of xml format.</param>
    /// <returns>The encrypted data.</returns>
    public static string Encrypt(byte[] dataBytes, string xmlPublicKey)
    {
        var bytes = EncryptBytes(dataBytes, xmlPublicKey);

        return Convert.ToBase64String(bytes);
    }

    public static byte[] DecryptBytes(byte[] data, string xmlPrivateKey)
    {
        data.Should().NotBeNull();
        xmlPrivateKey.Should().NotBeNullOrEmpty();

        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(xmlPrivateKey);
            byte[] bytes = rsa.Decrypt(data, false);
            return bytes;
        }
    }

    /// <summary>
    /// Decrypt string data with xml format.
    /// </summary>
    /// <param name="data">The data to be encrypted.</param>
    /// <param name="xmlPrivateKey">The private key of xml format.</param>
    /// <param name="encoding">The <see cref="T:System.Text.Encoding"/>,default is Encoding.UTF8.</param>
    /// <returns>The decrypted data.</returns>
    public static string Decrypt(string data, string xmlPrivateKey, Encoding encoding)
    {
        data.Should().NotBeNull();
        xmlPrivateKey.Should().NotBeNullOrEmpty();
        encoding.Should().NotBeNull();

        byte[] dataBytes = Convert.FromBase64String(data);

        var bytes = DecryptBytes(dataBytes, xmlPrivateKey);
        return encoding.GetString(bytes);
    }

    #endregion
}