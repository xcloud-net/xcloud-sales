using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using XCloud.Platform.Application.Messenger.Connection;

namespace XCloud.Platform.Application.Messenger.Utils;

[ExposeServices(typeof(MessengerUtils))]
public class MessengerUtils : ISingletonDependency
{
    private readonly IClock _clock;

    public MessengerUtils(IClock clock)
    {
        _clock = clock;
    }

    public bool IsInactive(IConnection connection)
    {
        return !connection.IsActive || connection.ClientIdentity.PingTime < this._clock.Now.AddMinutes(-1);
    }
}