using Volo.Abp.Application.Dtos;

namespace XCloud.Core.Dto;

public class TokenModel : IEntityDto
{
    public TokenModel()
    {
        //
    }

    public virtual string UserId { get; set; }

    public virtual string ExtData { get; set; }

    public virtual string AccessToken { get; set; }

    public virtual string RefreshToken { get; set; }
        
    public virtual DateTime ExpireUtc { get; set; }
}