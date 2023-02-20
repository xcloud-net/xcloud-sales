using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Member.Application.Service.Admin;

namespace XCloud.Platform.Member.Application.Service.User;

public class QueryUserConnectionRequest : IEntityDto
{
    public string UserId { get; set; }
    public string Platform { get; set; }
    public string AppId { get; set; }
}

public class UserAuthResponse : ApiResponse<SysUserDto>
{
    public bool IsNotActive { get; set; }
    public bool IsDeleted { get; set; }
}

public static class ThirdPartyPlatforms
{
    public static string WxMp = "wx-mp";
    public static string WxOpen = "wx-open";
    public static string WxUnion = "wx-union";
}

public class SysUserExternalAccessTokenDto : SysExternalAccessToken, IEntityDto
{
    //
}

public class SysUserExternalConnectDto : SysExternalConnect, IEntityDto
{
    //
}

public class GetValidUserAccessTokenInput : IEntityDto
{
    public string UserId { get; set; }
    public string AppId { get; set; }
    public string Platform { get; set; }
}

public class GetValidClientAccessTokenInput : IEntityDto
{
    public string AppId { get; set; }
    public string Platform { get; set; }
}

public class UserPhoneBindSmsMessage : IEntityDto
{
    public string Phone { get; set; }
    public string Code { get; set; }
}

public class UserMobilePhoneDto : IEntityDto
{
    public string UserId { get; set; }
    public string MobilePhone { get; set; }
    public bool? MobilePhoneConfirmed { get; set; }
}

public class SetIdCardInput : IEntityDto
{
    public string UserId { get; set; }
    public string IdCard { get; set; }
    public string RealName { get; set; }
}

public class SetUserMobileInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Mobile { get; set; }
}

public class ResetUserPasswordInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Password { get; set; }
}

public class UpdateUserStatusDto : IdDto
{
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class UpdateUserAvatarDto : IEntityDto<string>
{
    public string Id { get; set; }
    public string AvatarUrl { get; set; }
}

public class OAuthCodeDto : IEntityDto
{
    public string Code { get; set; }
    public bool? UseWechatProfile { get; set; }
}

public class UserCodeLoginDto : IEntityDto
{
    public string Code { get; set; }
    public string NickName { get; set; }
    public string AvatarUrl { get; set; }
}

public class AttachUserDataInput : IEntityDto
{
    public bool Mobile { get; set; }
}

public class QueryUserAccountInput : IEntityDto
{
    public string AccountIdentity { get; set; }
}

public class QueryUserDto : PagedRequest
{
    public int? Take { get; set; }
    public string Keyword { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsActive { get; set; }
}

public class SysUserDto : SysUser, IEntityDto
{
    public string UserId => this.Id;
    public string AccountMobile { get; set; }

    public SysAdminDto AdminIdentity { get; set; }
}