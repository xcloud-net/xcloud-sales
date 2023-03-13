using XCloud.Logging;
using XCloud.Sales.Application;

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
        await this.SalesEventBusService.NotifyClearActivityLog();
    }
}