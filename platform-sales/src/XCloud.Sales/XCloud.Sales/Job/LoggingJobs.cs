using XCloud.Logging;

namespace XCloud.Sales.Job;

[LogExceptionSilence]
[UnitOfWork]
public class LoggingJobs : SalesAppService, ITransientDependency
{
    public LoggingJobs()
    {
        //
    }

    [LogExceptionSilence]
    public virtual async Task TriggerCleanExpiredLogsAsync()
    {
        await this.EventBusService.NotifyClearActivityLog();
    }
}