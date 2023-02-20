using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Common.Application.Service.Token;

public class ValidationCode : IEntityDto
{
    public string Key { get; set; }
    public string Code { get; set; }
    public DateTime CreateTime { get; set; }
}

public class TokenDto : IEntityDto
{
    public TokenDto() { }
    public TokenDto(string token)
    {
        Token = token;
    }

    public string Token { get; set; }

    public static implicit operator TokenDto(string token)
    {
        return new TokenDto()
        {
            Token = token
        };
    }
}