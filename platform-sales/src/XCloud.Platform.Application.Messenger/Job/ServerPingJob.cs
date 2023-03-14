using XCloud.Core.DependencyInjection;
using XCloud.Core.Extension;
using XCloud.Platform.Application.Messenger.Registry;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Job;

public class ServerPingJob : IAutoRegistered
{
    private readonly IMessengerServer _messengerServer;
    private readonly IUserService _userService;
    private readonly IServiceProvider _provider;
    private readonly IRegistrationProvider _registrationProvider;
    public ServerPingJob(IMessengerServer messengerServer, IUserService userService, IServiceProvider provider, IRegistrationProvider registrationProvider)
    {
        this._messengerServer = messengerServer;
        this._userService = userService;
        this._provider = provider;
        this._registrationProvider = registrationProvider;
    }

    public async Task ExecuteAsync()
    {
        using var s = this._provider.CreateScope();

        var userUids = this._messengerServer.ConnectionManager.AsReadOnlyList().Select(x => x.ClientIdentity.SubjectId).WhereNotEmpty().ToArray();

        var groupUids = await _userService.GetUsersGroupsAsync(userUids);
        groupUids = groupUids.Distinct().ToArray();

        foreach (var g in groupUids)
        {
            var info = new GroupRegistrationInfo()
            {
                GroupId = g,
                ServerInstance = this._messengerServer.ServerInstanceId,
                Payload = new GroupRegistrationInfoPayload() { }
            };
            await this._registrationProvider.RegisterGroupInfoAsync(info);
        }
    }
}