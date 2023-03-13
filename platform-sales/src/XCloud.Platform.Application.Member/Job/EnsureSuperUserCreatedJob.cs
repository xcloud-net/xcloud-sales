using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Member.Job;

[UnitOfWork]
[ExposeServices(typeof(EnsureSuperUserCreatedJob))]
public class EnsureSuperUserCreatedJob : PlatformApplicationService
{
    private readonly EnsureSuperUserCreatedService _ensureSuperUserCreatedService;

    public EnsureSuperUserCreatedJob(EnsureSuperUserCreatedService ensureSuperUserCreatedService)
    {
        _ensureSuperUserCreatedService = ensureSuperUserCreatedService;
    }

    public virtual async Task EnsureSuperUserCreatedAsync()
    {
        using var uow = this.UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
        try
        {
            await this._ensureSuperUserCreatedService.EnsureSuperUserCreatedAsync();

            await uow.CompleteAsync();
        }
        catch (UserFriendlyException e)
        {
            await uow.RollbackAsync();
            this.Logger.LogWarning(message: e.Message, exception: e);
        }
        catch (Exception e)
        {
            await uow.RollbackAsync();
            this.Logger.LogError(message: e.Message, exception: e);
        }
    }
}