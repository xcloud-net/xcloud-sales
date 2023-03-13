using System.IO;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using XCloud.Application.Storage;

namespace XCloud.Platform.Application.Common.Service.Storage.QCloud;

public class TencentCloudBlobProvider : BlobProviderBase, ITransientDependency
{
    private readonly IFilePathCalculator _filePathCalculator;

    public TencentCloudBlobProvider(IFilePathCalculator filePathCalculator)
    {
        this._filePathCalculator = filePathCalculator;
    }

    public TencentCloudBlobProviderConfiguration GetTencentCloudConfiguration(BlobContainerConfiguration containerConfiguration)
    {
        return new TencentCloudBlobProviderConfiguration(containerConfiguration);
    }

    public override async Task SaveAsync(BlobProviderSaveArgs args)
    {
        var containerName = this.GetContainerName(args);
        var blobName = this.GetStorageKeyShardingPath(args.BlobName);
        var client = GetClient(args);

        if (!args.OverrideExisting && await client.CheckObjectIsExistAsync(containerName, blobName))
        {
            throw new BlobAlreadyExistsException(
                $"Saving BLOB '{args.BlobName}' does already exists in the container '{GetContainerName(args)}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
        }

        await client.UploadObjectAsync(GetContainerName(args), blobName, args.BlobStream);
    }

    public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
    {
        var blobName = this.GetStorageKeyShardingPath(args.BlobName);
        var containerName = GetContainerName(args);
        var client = GetClient(args);

        if (!await client.CheckObjectIsExistAsync(containerName, blobName))
        {
            return false;
        }

        await client.DeleteObjectAsync(containerName, blobName);

        return true;
    }

    public override async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
    {
        var client = this.GetClient(args);
        var containerName = this.GetContainerName(args);
        var blobName = this.GetStorageKeyShardingPath(args.BlobName);
        return await client.CheckObjectIsExistAsync(containerName, blobName);
    }

    public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
    {
        var blobName = this.GetStorageKeyShardingPath(args.BlobName);
        var containerName = GetContainerName(args);
        var client = GetClient(args);

        var stream = await client.DownloadObjectOrNullAsync(containerName, blobName);

        return stream;
    }

    protected virtual CosServerWrapObject GetClient(BlobProviderArgs args)
    {
        var configuration = this.GetTencentCloudConfiguration(args.Configuration);
        return new CosServerWrapObject(configuration);
    }

    protected virtual string GetStorageKeyShardingPath(string name)
    {
        var blobName = this._filePathCalculator.GetStorageKeyShardingPath(name);
        return blobName;
    }

    protected virtual string GetContainerName(BlobProviderArgs args)
    {
        var configuration = this.GetTencentCloudConfiguration(args.Configuration);
        return configuration.ContainerName.IsNullOrWhiteSpace()
            ? args.ContainerName
            : $"{configuration.ContainerName}";
    }
}