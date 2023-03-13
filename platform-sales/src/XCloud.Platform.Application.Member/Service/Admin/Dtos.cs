using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.Security;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Shared.Entity;

namespace XCloud.Platform.Application.Member.Service.Admin;

public class AdminAuthResponse : ApiResponse<SysAdminDto>
{
    public bool IsNotActive { get; set; }
}

public class CreateAdminDto : IHasUserId, IEntityDto
{
    public CreateAdminDto() { }
    public CreateAdminDto(string userId)
    {
        this.UserId = userId;
    }

    public string UserId { get; set; }
}

public class UpdateAdminStatusDto : IdDto
{
    public bool? IsActive { get; set; }
    public bool? IsSuperAdmin { get; set; }
}

public class QueryAdminDto : PagedRequest, IEntityDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class SysAdminDto : SysAdmin, IEntityDto
{
    public string[] PermissionKeys { get; set; }

    public SysRoleDto[] Roles { get; set; }

    public string AdminId => this.Id;

    public SysUserDto SysUser { get; set; }

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
    /// 头像链接
    /// </summary>
    public string Avatar { get; set; }
}