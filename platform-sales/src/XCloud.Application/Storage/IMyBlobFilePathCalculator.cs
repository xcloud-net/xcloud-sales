using System;
using System.Linq;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace XCloud.Application.Storage;

[Obsolete("optimise required")]
public interface IMyBlobFilePathCalculator
{
    string[] GetStorageKeyShardingPathArray(string key);
    
    string GetStorageKeyShardingPath(string key, string joiner = "/");
}

[ExposeServices(typeof(IMyBlobFilePathCalculator))]
public class MyBlobFilePathCalculator : IMyBlobFilePathCalculator, ISingletonDependency
{
    public MyBlobFilePathCalculator() { }

    /// <summary>
    /// 按照key设计一个规则把文件分散到不同的文件夹或者空间里
    /// </summary>
    public string[] GetStorageKeyShardingPathArray(string key)
    {
        var fileName = key.Split('.').First();

        fileName = new StringBuilder().Append(fileName.Where(x => x != ' ').ToArray()).ToString();

        var prefix = fileName.PadRight(10, '0').Take(3).Select(x => x.ToString()).ToArray();

        return prefix;
    }

    public string GetStorageKeyShardingPath(string key, string joiner = "/")
    {
        var arr = GetStorageKeyShardingPathArray(key);
        arr = arr.Append(key).ToArray();

        var path = string.Join(joiner, arr);
        return path;
    }
}