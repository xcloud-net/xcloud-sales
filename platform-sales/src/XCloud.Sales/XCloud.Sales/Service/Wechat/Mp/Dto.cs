using Volo.Abp.Application.Dtos;

namespace XCloud.Sales.Service.Wechat.Mp;

public class WechatProfileDto : IEntityDto
{
    public string OpenId { get; set; }
    public string UnionId { get; set; }
}