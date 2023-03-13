using XCloud.Platform.Core.Domain.Storage;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Application.Common.Extension;

public static class PlatformCommonExtension
{
    public static StorageMetaDto ToDto(this StorageResourceMeta entity)
    {
        return new StorageMetaDto()
        {
            Id = entity.Id,
            ContentType = entity.ContentType,
            ResourceSize = entity.ResourceSize,
            ResourceKey = entity.ResourceKey,
            ResourceHash = entity.ResourceHash,
            HashType = entity.HashType,
            ExtraData = entity.ExtraData,
            StorageProvider = entity.StorageProvider,
        };
    }
}