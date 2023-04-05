using XCloud.Platform.Application.Member.Service.User;

namespace XCloud.Platform.Application.Member.Extension;

public static class PlatformMemberExtension
{
    public static SysUserDto HideSensitiveInformation(this SysUserDto user)
    {
        user.PassWord = null;
        return user;
    }
}