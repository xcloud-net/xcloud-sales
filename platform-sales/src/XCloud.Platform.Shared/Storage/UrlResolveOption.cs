using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Shared.Storage;

public class UrlResolveOption : IEntityDto
{
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? CompressLevel { get; set; }
}