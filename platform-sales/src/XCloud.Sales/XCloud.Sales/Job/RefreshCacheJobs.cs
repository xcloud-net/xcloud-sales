using XCloud.Logging;
using XCloud.Sales.Application;
using XCloud.Sales.ViewService;

namespace XCloud.Sales.Job;

[LogExceptionSilence]
[UnitOfWork]
public class RefreshCacheJobs : SalesAppService, ITransientDependency
{
    private readonly IHomeViewService _homeViewService;
    private readonly ICategoryViewService _categoryViewService;

    public RefreshCacheJobs(IHomeViewService homeViewService, ICategoryViewService categoryViewService)
    {
        this._homeViewService = homeViewService;
        this._categoryViewService = categoryViewService;
    }
    
    [LogExceptionSilence]
    public virtual async Task RefreshViewCacheAsync()
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource:"refresh-view-cache-lock",
            expiryTime: TimeSpan.FromSeconds(10));
        if (dlock.IsAcquired)
        {
            await this._homeViewService.QueryHomePageDtoAsync(new CachePolicy() { Refresh = true });
            await this._categoryViewService.QueryCategoryPageDataAsync(new CachePolicy() { Refresh = true});
        }
        else
        {
            this.Logger.LogWarning("fail to get lock to refresh view cache");
        }
    }
}