using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using XCloud.Application.Storage;
using XCloud.Platform.Common.Application.Configuration;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Storage;
using XCloud.Platform.Shared.Dto;
using XCloud.Redis;

namespace XCloud.Platform.Common.Application.Service.Storage;

public interface IStorageService : IApplicationService
{
    string StorageProviderName { get; }

    IBlobContainer BlobContainer { get; }

    Task<StorageResourceMeta> UploadStreamAsync(FileUploadStreamArgs args);

    Task<Stream> GetFileStreamOrNullAsync(string fileKey);
}

public class StorageService : PlatformApplicationService, IStorageService
{
    private readonly StorageToolbox _storageToolbox;

    public StorageService(StorageToolbox storageToolbox)
    {
        _storageToolbox = storageToolbox;
        this._lazyBlobContainer = new Lazy<IBlobContainer>(this.CreateBlobContainer);
    }

    private IBlobContainer CreateBlobContainer()
    {
        var name = this.StorageProviderName;
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(this.StorageProviderName));

        var container = this._storageToolbox.BlobContainerFactory.Create(name);

        return container;
    }

    private string GetProviderNameOrDefault(string deft)
    {
        var name = this.Configuration.GetDefaultStorageProvider();

        if (string.IsNullOrWhiteSpace(name))
            return deft;

        return name;
    }

    private readonly Lazy<IBlobContainer> _lazyBlobContainer;

    public virtual IBlobContainer BlobContainer => this._lazyBlobContainer.Value;

    public virtual string StorageProviderName => this.GetProviderNameOrDefault(StorageProviders.AbpFsBlobProvider1);

    private async Task<StorageResourceMeta> PrepareResourceMetaModel(FileUploadStreamArgs args, ITempFile tempFileKey)
    {
        var model = new StorageResourceMeta(this.StorageProviderName)
        {
            ContentType = args.ContentType,
            FileExtension = this._storageToolbox.StorageHelper.GetNormalizedFileExtension(args.FileName),
            //length
            ResourceSize = await this._storageToolbox.TempFileService.GetTempFileSize(tempFileKey)
        };
        //hash
        using var streamForHash = await this._storageToolbox.TempFileService.OpenTempFileStreamAsync(tempFileKey);
        model.ResourceHash = this._storageToolbox.FileHashProvider.CalculateFileHashString(streamForHash);
        model.HashType = this._storageToolbox.FileHashProvider.HashType;
        //file key
        model.ResourceKey = $"{model.ResourceHash}.{model.FileExtension.TrimStart('.')}";

        var extraObject = new
        {
            //
        };
        model.ExtraData = this.JsonDataSerializer.SerializeToString(extraObject);

        return model;
    }

    public async Task<StorageResourceMeta> UploadStreamAsync(FileUploadStreamArgs args)
    {
        if (args == null)
            throw new ArgumentNullException(nameof(args));
        if (args.Stream == null)
            throw new ArgumentNullException(nameof(args.Stream));
        if (string.IsNullOrWhiteSpace(args.FileName))
            throw new ArgumentNullException(nameof(args.FileName));

        var tempFileKey = await this._storageToolbox.TempFileService.CreateTempFileAsync(args.Stream);
        try
        {
            var model = await this.PrepareResourceMetaModel(args, tempFileKey);

            using var @lock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
                resource: $"{nameof(StorageService)}-save-resource:{model.ResourceKey}",
                expiryTime: TimeSpan.FromSeconds(10),
                waitTime: TimeSpan.FromSeconds(3),
                retryTime: TimeSpan.FromSeconds(1));

            if (@lock.IsAcquired)
            {
                if (!await BlobContainer.ExistsAsync(model.ResourceKey))
                {
                    //上传文件
                    using var tempFileStream =
                        await this._storageToolbox.TempFileService.OpenTempFileStreamAsync(tempFileKey);
                    await BlobContainer.SaveAsync(model.ResourceKey, tempFileStream);
                }

                //如果之前有人上传过就直接返回
                var savedModel = await this._storageToolbox.StorageMetaService.TrySaveResourceMetaAsync(model);
                return savedModel;
            }
            else
            {
                throw new FailToGetRedLockException("upload concurrency exception");
            }
        }
        finally
        {
            await this._storageToolbox.TempFileService.DeleteTempFileAsync(tempFileKey);
        }
    }

    public async Task<Stream> GetFileStreamOrNullAsync(string fileKey)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
            throw new ArgumentNullException(nameof(fileKey));

        var stream = await BlobContainer.GetOrNullAsync(fileKey);
        return stream;
    }
}