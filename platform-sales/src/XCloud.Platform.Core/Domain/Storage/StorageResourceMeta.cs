using Volo.Abp;
using Volo.Abp.Auditing;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Core.Domain.Storage;

/// <summary>
/// 系统文件池，只增不减，可以由迁移工具或者定时任务迁移
/// </summary>
public class StorageResourceMeta : EntityBase, IPlatformEntity, ISoftDelete, IHasModificationTime
{
    public StorageResourceMeta() { }
    public StorageResourceMeta(string provider) : this()
    {
        this.StorageProvider = provider;
    }

    public string FileExtension { get; set; }

    public string ContentType { get; set; }

    public long ResourceSize { get; set; }

    public string ResourceKey { get; set; }

    public string ResourceHash { get; set; }

    public string HashType { get; set; }

    public string ExtraData { get; set; }

    public string StorageProvider { get; set; } = StorageProviders.None;

    public int ReferenceCount { get; set; }

    public int UploadTimes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? LastModificationTime { get; set; }
}