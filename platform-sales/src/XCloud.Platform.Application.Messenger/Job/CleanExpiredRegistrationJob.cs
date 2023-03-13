using XCloud.Core.DependencyInjection;

namespace XCloud.Platform.Application.Messenger.Job;

public class CleanExpiredRegistrationJob : IAutoRegistered
{
    public async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}