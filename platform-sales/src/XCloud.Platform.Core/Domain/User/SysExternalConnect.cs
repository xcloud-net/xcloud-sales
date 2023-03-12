using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.User;

public class SysExternalConnect : EntityBase, IMemberEntity
{
    public string UserId { get; set; }

    /// <summary>
    /// wechat-mini/wechat-mp/alipay/others
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// appid
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// oauth openid
    /// </summary>
    public string OpenId { get; set; }
}