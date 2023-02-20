using System.Threading.Tasks;
using XCloud.Core.DependencyInjection;
using XCloud.Platform.Common.Application.Service.Messenger.Connection;
using XCloud.Platform.Common.Application.Service.Messenger.UserContext;

namespace XCloud.Platform.Common.Application.Service.Messenger.Jobs;

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