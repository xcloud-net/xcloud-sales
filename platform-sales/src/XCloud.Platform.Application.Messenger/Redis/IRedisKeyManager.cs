using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Messenger.Redis;

public interface IRedisKeyManager
{
    string UserRegInfoKey(string userUid);
    
    string UserDeviceRegHashKey(string device);
    
    string GroupRegInfoKey(string groupUid);
    
    string GroupServerHashKey(string serverId);
}

[ExposeServices(typeof(IRedisKeyManager))]
public class DefaultRedisKeyManager : IRedisKeyManager, ISingletonDependency
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