using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.User;

public enum UserIdentityTypeEnum : int
{
    Email = 2,
    MobilePhone = 3
}

/// <summary>
/// 用于identity一个用户
/// </summary>
public class SysUserIdentity : EntityBase, IMemberEntity
{
    /// <summary>
    /// 用户id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 用户名|邮箱|手机|openid
    /// </summary>
    public string UserIdentity { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// 类型
    /// </summary>
    public int IdentityType { get; set; }

    public string MobilePhone { get; set; }
    /// <summary>
    /// 手机区号
    /// </summary>
    public string MobileAreaCode { get; set; }
    public bool? MobileConfirmed { get; set; }
    public DateTime? MobileConfirmedTimeUtc { get; set; }

    public string Email { get; set; }
    public bool? EmailConfirmed { get; set; }
    public DateTime? EmailConfirmedTimeUtc { get; set; }
}