using Volo.Abp.DependencyInjection;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Common.Application.Service.Storage;

public interface IQCloudStorageService : IStorageService
{
    //
}

[ExposeServices(typeof(IQCloudStorageService))]
public class QCloudStorageService : StorageService, IQCloudStorageService
{
    public QCloudStorageService(StorageToolbox storageToolbox) : base(storageToolbox)
    {
        //
    }

    public override string StorageProviderName => StorageProviders.QCloudBlobProvider1;
}