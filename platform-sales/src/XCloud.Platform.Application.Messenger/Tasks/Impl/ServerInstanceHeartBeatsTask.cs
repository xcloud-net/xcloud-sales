using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Application.Messenger.Service;
using XCloud.Platform.Application.Messenger.Utils;

namespace XCloud.Platform.Application.Messenger.Tasks.Impl;

public class ServerInstanceHeartBeatsTask : IMessengerTask, IScopedDependency
{
    private readonly IServerInstanceService _serverInstanceService;
    private readonly IMessengerServer _messengerServer;
    private readonly IClock _clock;
    private readonly MessengerUtils _messengerUtils;

    public ServerInstanceHeartBeatsTask(IServerInstanceService serverInstanceService, IMessengerServer messengerServer,
        IClock clock, MessengerUtils messengerUtils)
    {
        _serverInstanceService = serverInstanceService;
        _messengerServer = messengerServer;
        _clock = clock;
        _messengerUtils = messengerUtils;
    }

    public TimeSpan Delay => TimeSpan.FromMinutes(1);

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var deadConnections = this._messengerServer.ConnectionManager.AsReadOnlyList()
            .Where(x => this._messengerUtils.IsInactive(x)).ToArray();
        foreach (var connection in deadConnections)
        {
            using (connection)
            {
                //
            }

            this._messengerServer.ConnectionManager.RemoveConnection(connection);
        }
        
        
        await this._serverInstanceService.RegAsync(new ServerInstanceDto()
        {
            InstanceId = this._messengerServer.ServerInstanceId,
            ConnectionCount = this._messengerServer.ConnectionManager.AsReadOnlyList().Count,
            PingTime = this._clock.Now
        });
    }
}