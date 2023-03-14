using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Registry;

namespace XCloud.Platform.Application.Messenger.Extension;

public static class MessengerExtension
{
    public static UserRegistrationInfo ToRegInfo(this WsConnection con)
    {
        var res = new UserRegistrationInfo()
        {
            UserId = con.ClientIdentity.SubjectId,
            DeviceType = con.ClientIdentity.DeviceType,
            Payload = new UserRegistrationInfoPayload()
            {
                ServerInstanceId = con.Server.ServerInstanceId,
                PingTimeUtc = DateTime.UtcNow
            }
        };
        return res;
    }
}