using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Shared.Dto;

public static class StorageProviders
{
    public static string None => "none";
    public static string OriginUrl => "origin-url";
    public static string AbpFsBlobProvider1 => "file";
    public static string QCloudBlobProvider1 => "qcloud";
}

/// <summary>
/// resource object
/// </summary>
public class StorageMetaDto : IEntityDto<string>
{
    public StorageMetaDto()
    {
        //
    }

    public StorageMetaDto(string provider) : this()
    {
        this.StorageProvider = provider;
    }

    public string Id { get; set; }

    public string Url { get; set; }

    public string ContentType { get; set; }

    public long ResourceSize { get; set; }

    public string ResourceKey { get; set; }

    public string ResourceHash { get; set; }

    public string HashType { get; set; }

    public string ExtraData { get; set; }

    public string StorageProvider { get; set; }
}