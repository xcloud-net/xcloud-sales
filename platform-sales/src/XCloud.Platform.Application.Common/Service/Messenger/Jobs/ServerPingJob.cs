using System.Threading.Tasks;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Extension;
using XCloud.Platform.Application.Common.Service.Messenger.Connection;
using XCloud.Platform.Application.Common.Service.Messenger.RegistrationCenter;
using XCloud.Platform.Application.Common.Service.Messenger.UserContext;

namespace XCloud.Platform.Application.Common.Service.Messenger.Jobs;

public class ServerPingJob : IAutoRegistered
{
    private readonly IWsServer _wsServer;
    private readonly IUserGroups _userGroups;
    private readonly IServiceProvider _provider;
    private readonly IRegistrationProvider _registrationProvider;
    public ServerPingJob(IWsServer wsServer, IUserGroups userGroups, IServiceProvider provider, IRegistrationProvider registrationProvider)
    {
        this._wsServer = wsServer;
        this._userGroups = userGroups;
        this._provider = provider;
        this._registrationProvider = registrationProvider;
    }

    public async Task ExecuteAsync()
    {
        using var s = this._provider.CreateScope();

        var userUids = this._wsServer.ClientManager.AllConnections().Select(x => x.Client.SubjectId).WhereNotEmpty().ToArray();

        var groupUids = await _userGroups.GetUsersGroupsAsync(userUids);
        groupUids = groupUids.Distinct().ToArray();

        foreach (var g in groupUids)
        {
            var info = new GroupRegistrationInfo()
            {
                GroupId = g,
                ServerInstance = this._wsServer.ServerInstanceId,
                Payload = new GroupRegistrationInfoPayload() { }
            };
            await this._registrationProvider.RegisterGroupInfoAsync(info);
        }
    }
}