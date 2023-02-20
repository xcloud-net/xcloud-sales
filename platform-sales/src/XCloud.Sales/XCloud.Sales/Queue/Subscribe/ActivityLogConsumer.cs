using DotNetCore.CAP;
using XCloud.Logging;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Services.Logging;

namespace XCloud.Sales.Queue.Subscribe;

[LogExceptionSilence]
[UnitOfWork]
public class ActivityLogConsumer : SalesAppService, ICapSubscribe
{
    private readonly IActivityLogService _activityLogService;

    public ActivityLogConsumer(IActivityLogService activityLogService)
    {
        this._activityLogService = activityLogService;
    }

    [CapSubscribe(SalesMessageTopics.ClearAllActivityLogs)]
    public virtual async Task ClearAllActivityLogs(string letThisEmpty)
    {
        await this._activityLogService.ClearExpiredDataWithLockAsync(this.Clock.Now.AddMonths(-3));
    }

    [CapSubscribe(SalesMessageTopics.InsertActivityLog)]
    public virtual async Task InsertActivityLog(ActivityLog log)
    {
        log = await this._activityLogService.TryResolveGeoAddressAsync(log);
        await this._activityLogService.InsertAsync(log);
    }
}