using System.Collections.Generic;
using System.IO;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using XCloud.Application.Storage;
using XCloud.Core.Extension;

namespace XCloud.Platform.Application.Common.Service.Storage.AbpFileSystem;

public class BlobFileKeyShardingPathCalculator : IBlobFilePathCalculator
{
    private readonly IFilePathCalculator _filePathCalculator;
    public BlobFileKeyShardingPathCalculator(IFilePathCalculator filePathCalculator)
    {
        this._filePathCalculator = filePathCalculator;
    }

    public string Calculate(BlobProviderArgs args)
    {
        var config = args.Configuration.GetFileSystemConfiguration();

        var list = new List<string>() { config.BasePath };
        if (config.AppendContainerNameToBasePath)
        {
            list.Add(args.ContainerName);
        }

        var prefix = this._filePathCalculator.GetStorageKeyShardingPathArray(args.BlobName);

        list.AddRange(prefix);

        list.Add(args.BlobName);

        var res = Path.Combine(list.WhereNotEmpty().ToArray());

        return res;
    }
}