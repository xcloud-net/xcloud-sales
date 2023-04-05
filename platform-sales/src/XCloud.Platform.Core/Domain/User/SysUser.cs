using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared.Entity;

namespace XCloud.Platform.Core.Domain.User;

public class SysUser : EntityBase, IMemberEntity, IAccount
{
    public SysUser() { }

    public SysUser(string id)
    {
        this.Id = id;
    }

    /// <summary>
    /// 用于登陆，唯一标志
    /// </summary>
    public string IdentityName { get; set; }

    public string OriginIdentityName { get; set; }

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

    public DateTime? LastPasswordUpdateTime { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Gender { get; set; } = (int)GenderEnum.Unknow;

    public bool IsActive { get; set; }


    public bool IsDeleted { get; set; }

    public DateTime? DeletionTime { get; set; }

    public DateTime? LastModificationTime { get; set; }
}