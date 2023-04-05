using Hangfire;
using XCloud.Platform.Application.Common.Job;

namespace XCloud.Platform.Application.Common.Configuration;

public static class JobExtension
{
    public static void StartPlatformCommonJobs(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<AppRegisterJob>("app-register-job",
            x => x.HandleRegister(), Cron.Minutely());
    }
}