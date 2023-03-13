using XCloud.Core.DependencyInjection;
using XCloud.Platform.Application.Messenger.Connection;
using XCloud.Platform.Application.Messenger.Server;
using XCloud.Platform.Application.Messenger.Service;

namespace XCloud.Platform.Application.Messenger.Job;

public class ServerCleanupJob : IAutoRegistered
{
    private readonly IWsServer wsServer;
    private readonly IUserGroups userGroups;
    private readonly IServiceProvider provider;
    public ServerCleanupJob(IWsServer wsServer, IUserGroups userGroups, IServiceProvider provider)
    {
        this.wsServer = wsServer;
        this.userGroups = userGroups;
        this.provider = provider;
    }

    public async Task ExecuteAsync()
    {
        await this.wsServer.Cleanup();
    }
}