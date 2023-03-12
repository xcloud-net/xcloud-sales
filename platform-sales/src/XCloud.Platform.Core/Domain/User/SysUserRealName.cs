using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.User;

public enum IdCardTypeEnum : int
{
    IdCard = 1,
    Passport = 2,
}

public class SysUserRealName : EntityBase, IMemberEntity
{
    public string IdCardIdentity { get; set; }

    public string UserId { get; set; }

    public string Data { get; set; }

    public int IdCardType { get; set; }

    /// <summary>
    /// 身份证
    /// </summary>
    public string IdCard { get; set; }

    /// <summary>
    /// 身份证姓名
    /// </summary>
    public string IdCardRealName { get; set; }

    public string IdCardFrontUrl { get; set; }

    public string IdCardBackUrl { get; set; }

    public string IdCardHandUrl { get; set; }

    public DateTime? StartTimeUtc { get; set; }

    public DateTime? EndTimeUtc { get; set; }

    public int IdCardStatus { get; set; }

    /// <summary>
    /// 是否实名认证
    /// </summary>
    public bool? IdCardConfirmed { get; set; }

    public DateTime? IdCardConfirmedTimeUtc { get; set; }

    public string ConfirmerId { get; set; }
}