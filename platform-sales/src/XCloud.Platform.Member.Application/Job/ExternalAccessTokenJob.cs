using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Logging;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Redis;

namespace XCloud.Platform.Member.Application.Job;

[LogExceptionSilence]
[UnitOfWork]
[ExposeServices(typeof(ExternalAccessTokenJob))]
public class ExternalAccessTokenJob : PlatformApplicationService, ITransientDependency
{
    private readonly IExternalAccessTokenService _userExternalAccessTokenService;

    public ExternalAccessTokenJob(IExternalAccessTokenService userExternalAccessTokenService)
    {
        this._userExternalAccessTokenService = userExternalAccessTokenService;
    }

    [LogExceptionSilence]
    public virtual async Task CleanExpiredAccessTokenAsync()
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(ExternalAccessTokenJob)}.{nameof(CleanExpiredAccessTokenAsync)}",
            TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            await this._userExternalAccessTokenService.CleanExpiredTokenAsync();
        }
        else
        {
            throw new FailToGetRedLockException("clean expired access token:failed to get dlock");
        }
    }
}