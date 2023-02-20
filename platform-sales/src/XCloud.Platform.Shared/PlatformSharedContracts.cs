using System;

using Volo.Abp;
using Volo.Abp.Auditing;
using XCloud.Core.Application;

namespace XCloud.Platform.Shared;

/// <summary>
/// 账号
/// </summary>
public interface IAccount : IEntityBase, IHasIdentityNameFields,
    IHasCreationTime, ISoftDelete, IHasDeletionTime, IHasModificationTime
{
    /// <summary>
    /// 昵称，用于展示
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// md5加密的密码
    /// </summary>
    public string PassWord { get; set; }

    /// <summary>
    /// 头像链接
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 最后修改密码的时间
    /// </summary>
    public DateTime? LastPasswordUpdateTime { get; set; }
}

public interface IHasUserId
{
    string UserId { get; set; }
}

public interface IHasAppKey
{
    string AppKey { get; set; }
}