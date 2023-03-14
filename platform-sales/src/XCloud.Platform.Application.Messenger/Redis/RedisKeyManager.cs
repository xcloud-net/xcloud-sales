using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Messenger.Redis;

[ExposeServices(typeof(RedisKeyManager))]
public class RedisKeyManager : ISingletonDependency
{
    public string GroupRegInfoKey(string groupUid)
    {
        var res = $"im:reg:group:{groupUid}";
        return res;
    }

    public string GroupServerHashKey(string serverId)
    {
        var res = $"hash_{serverId}";
        return res;
    }

    public string UserDeviceRegHashKey(string device)
    {
        var res = $"hash_{device}";
        return res;
    }

    public string UserRegInfoKey(string userUid)
    {
        var res = $"im:reg:user:{userUid}";
        return res;
    }
}