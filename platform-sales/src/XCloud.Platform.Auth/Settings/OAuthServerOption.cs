using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Auth.Settings;

public class OAuthServerOption : IEntityDto
{
    public string Server { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Scope { get; set; }
}