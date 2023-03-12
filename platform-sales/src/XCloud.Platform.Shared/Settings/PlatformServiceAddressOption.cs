using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Shared.Settings;

public class PlatformServiceAddressOption : IEntityDto
{
    public string PublicGateway { get; set; }
    public string InternalGateway { get; set; }
    public string FrontEnd { get; set; }
}