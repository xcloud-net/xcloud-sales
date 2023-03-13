using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using XCloud.Platform.Application.Common.Service.App;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Common.Job;

[UnitOfWork]
[ExposeServices(typeof(AppRegisterJob))]
public class AppRegisterJob : PlatformApplicationService, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppService _appService;

    public AppRegisterJob(IServiceProvider serviceProvider,
        IAppService appService)
    {
        this._serviceProvider = serviceProvider;
        this._appService = appService;
    }

    public async Task HandleRegister()
    {
        using var distributedLock = await this.RedLockClient.RedLockFactory
            .CreateLockAsync(nameof(AppRegisterJob),
                expiryTime: TimeSpan.FromMinutes(1),
                waitTime: TimeSpan.FromSeconds(10),
                retryTime: TimeSpan.FromSeconds(1));

        if (distributedLock.IsAcquired)
        {
            var contributors = this._serviceProvider.GetServices<IAppContributor>().ToArray();

            if (!contributors.Any())
            {
                return;
            }

            var apps = await Task.WhenAll(contributors.Select(x => x.GetApp()));

            await this._appService.FlushAppInformation(apps);

            this.Logger.LogInformation($"finish-{this.Clock.Now}");
        }
        else
        {
            this.Logger.LogInformation($"未能获取分布式锁，放弃本次操作");
        }
    }
}