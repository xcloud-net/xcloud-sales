using System.Threading.Tasks;
using XCloud.Core.DependencyInjection;

namespace XCloud.Platform.Application.Common.Service.Messenger.Jobs;

public class CleanExpiredRegistrationJob : IAutoRegistered
{
    public async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}