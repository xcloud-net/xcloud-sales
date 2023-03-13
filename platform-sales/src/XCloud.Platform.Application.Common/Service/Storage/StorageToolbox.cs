using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using XCloud.Application.Storage;

namespace XCloud.Platform.Application.Common.Service.Storage;

[ExposeServices(typeof(StorageToolbox))]
public class StorageToolbox : ITransientDependency
{
    private readonly IAbpLazyServiceProvider _abpLazyServiceProvider;

    public StorageToolbox(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }

    public IStorageMetaService StorageMetaService =>
        this._abpLazyServiceProvider.LazyGetRequiredService<IStorageMetaService>();

    public IFileHashProvider FileHashProvider =>
        this._abpLazyServiceProvider.LazyGetRequiredService<IFileHashProvider>();

    public ITempFileService TempFileService =>
        this._abpLazyServiceProvider.LazyGetRequiredService<ITempFileService>();

    public StorageHelper StorageHelper =>
        this._abpLazyServiceProvider.LazyGetRequiredService<StorageHelper>();

    public IBlobContainerFactory BlobContainerFactory =>
        this._abpLazyServiceProvider.LazyGetRequiredService<IBlobContainerFactory>();
}