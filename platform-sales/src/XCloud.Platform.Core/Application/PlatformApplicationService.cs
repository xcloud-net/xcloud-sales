using XCloud.Core.Application;
using XCloud.Platform.Shared.Helper;
using XCloud.Redis.DistributedLock;

namespace XCloud.Platform.Core.Application;

public abstract class PlatformApplicationService : XCloudApplicationService
{
    protected MemberHelper MemberHelper => 
        this.LazyServiceProvider.LazyGetRequiredService<MemberHelper>();
    
    protected RedLockClient RedLockClient => LazyServiceProvider.LazyGetRequiredService<RedLockClient>();
}