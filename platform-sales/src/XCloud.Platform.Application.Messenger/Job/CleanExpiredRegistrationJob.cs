using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Messenger.Service;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Messenger.Job;

public class CleanExpiredRegistrationJob : PlatformApplicationService, IScopedDependency
{
    private readonly IUserRegistrationService _userRegistrationService;
    private readonly IServerInstanceService _serverInstanceService;

    public CleanExpiredRegistrationJob(IUserRegistrationService userRegistrationService,
        IServerInstanceService serverInstanceService)
    {
        _userRegistrationService = userRegistrationService;
        _serverInstanceService = serverInstanceService;
    }

    public async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}