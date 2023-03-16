using Volo.Abp.Application.Dtos;

namespace XCloud.Core.Dto;

public class AuthTokenDto : IEntityDto
{
    public AuthTokenDto()
    {
        //
    }

    public virtual string AccessToken { get; set; }

    public virtual string RefreshToken { get; set; }
        
    public virtual DateTime ExpiredTime { get; set; }
}