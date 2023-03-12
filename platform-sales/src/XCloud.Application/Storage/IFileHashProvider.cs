using System;
using System.IO;
using System.Security.Cryptography;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Configuration;

namespace XCloud.Application.Storage;

public interface IFileHashProvider
{
    string HashType { get; }
    string CalculateFileHashString(Stream stream);
    string CalculateFileHashString(byte[] bs);
}

[ExposeServices(typeof(IFileHashProvider))]
public class MD5FileHashProvider : IFileHashProvider, ISingletonDependency
{
    private readonly AppConfig appConfig;

    public MD5FileHashProvider(AppConfig appConfig)
    {
        this.appConfig = appConfig;
    }

    public string HashType => "md5";

    public string CalculateFileHashString(Stream stream)
    {
        using var md5Algorithm = MD5.Create();
        var bytes = md5Algorithm.ComputeHash(stream);
        var hash = BitConverter.ToString(bytes);

        hash = hash.Replace("-", string.Empty);

        return hash;
    }

    public string CalculateFileHashString(byte[] bs)
    {
        using var ms = new MemoryStream();
        ms.Write(bs, 0, bs.Length);

        return CalculateFileHashString(ms);
    }

}