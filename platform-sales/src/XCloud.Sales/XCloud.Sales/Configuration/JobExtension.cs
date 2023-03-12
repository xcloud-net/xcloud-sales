using Hangfire;
using Microsoft.Extensions.Options;
using XCloud.Core;
using XCloud.Sales.Core.Settings;
using XCloud.Sales.Job;

namespace XCloud.Sales.Configuration;

public static class JobExtension
{
    public static bool AutoStartSalesJob(this IServiceProvider serviceProvider)
    {
        using var s = serviceProvider.CreateScope();

        var option = s.ServiceProvider.GetRequiredService<IOptions<SalesJobOption>>().Value;
        if (option == null)
            throw new ConfigException(nameof(AutoStartSalesJob));

        return option.AutoStartJob;
    }

    public static void StartSalesJobs(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<OrderJobs>("cancel-unpaid-order", x => x.CancelUnpaidOrdersAsync(),
            Cron.Hourly());

        recurringJobManager.AddOrUpdate<OrderJobs>("auto-confirm-shipped-order", x => x.AutoConfirmShippedOrderAsync(),
            Cron.Hourly());

        recurringJobManager.AddOrUpdate<CategoryJobs>("mall-category-try-fix-tree",
            x => x.TryFixCategoryTreeAsync(), Cron.Daily());

        recurringJobManager.AddOrUpdate<UserJobs>("trigger-sync-user-info-from-platform",
            x => x.TriggerSyncUserInformationFromPlatformAsync(), Cron.Daily());

        recurringJobManager.AddOrUpdate<LoggingJobs>("trigger-clean-expired-logs",
            x => x.TriggerCleanExpiredLogsAsync(),
            Cron.Daily());

        recurringJobManager.AddOrUpdate<RefreshCacheJobs>("refresh-view-cache", x => x.RefreshViewCacheAsync(),
            Cron.Minutely());
    }
}