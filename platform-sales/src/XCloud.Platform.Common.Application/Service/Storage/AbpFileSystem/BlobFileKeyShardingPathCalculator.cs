using System.Collections.Generic;
using System.IO;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using XCloud.Application.Storage;
using XCloud.Core.Extension;

namespace XCloud.Platform.Common.Application.Service.Storage.AbpFileSystem;

public class BlobFileKeyShardingPathCalculator : IBlobFilePathCalculator
{
    private readonly IMyBlobFilePathCalculator _myBlobFilePathCalculator;
    public BlobFileKeyShardingPathCalculator(IMyBlobFilePathCalculator myBlobFilePathCalculator)
    {
        this._myBlobFilePathCalculator = myBlobFilePathCalculator;
    }

    public string Calculate(BlobProviderArgs args)
    {
        var config = args.Configuration.GetFileSystemConfiguration();

        var list = new List<string>() { config.BasePath };
        if (config.AppendContainerNameToBasePath)
        {
            list.Add(args.ContainerName);
        }

        var prefix = this._myBlobFilePathCalculator.GetStorageKeyShardingPathArray(args.BlobName);

        list.AddRange(prefix);

        list.Add(args.BlobName);

        var res = Path.Combine(list.WhereNotEmpty().ToArray());

        return res;
    }
}