using Hangfire;
using XCloud.Platform.Common.Application.Job;

namespace XCloud.Platform.Common.Application.Configuration;

public static class JobExtension
{
    public static void StartPlatformCommonJobs(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<AppRegisterJob>("app-register-job",
            x => x.HandleRegister(), Cron.Minutely());
    }
}