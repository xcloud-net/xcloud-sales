using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Extension;

public static class MessengerExtension
{
    public static SysUserOnlineStatusDto ToRegInfo(this IConnection con)
    {
        var res = new SysUserOnlineStatusDto()
        {
            UserId = con.ClientIdentity.SubjectId,
            DeviceId = con.ClientIdentity.DeviceType,
            ServerInstanceId = con.Server.ServerInstanceId,
        };
        return res;
    }
}