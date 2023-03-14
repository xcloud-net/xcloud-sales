using XCloud.Core.DependencyInjection;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Job;

public class ServerCleanupJob : IAutoRegistered
{
    private readonly IMessengerServer _messengerServer;
    private readonly IUserService _userService;
    private readonly IServiceProvider provider;
    public ServerCleanupJob(IMessengerServer messengerServer, IUserService userService, IServiceProvider provider)
    {
        this._messengerServer = messengerServer;
        this._userService = userService;
        this.provider = provider;
    }

    public async Task ExecuteAsync()
    {
        await this._messengerServer.CleanupAsync();
    }
}