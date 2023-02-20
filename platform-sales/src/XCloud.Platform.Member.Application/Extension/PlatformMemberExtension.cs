using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Member.Application.Extension;

public static class PlatformMemberExtension
{
    public static SysUserDto HideSensitiveInformation(this SysUserDto user)
    {
        user.PassWord = null;
        return user;
    }
}