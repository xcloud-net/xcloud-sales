using Hangfire;
using XCloud.Platform.Member.Application.Job;

namespace XCloud.Platform.Member.Application.Configuration;

public static class JobExtension
{
    public static void StartPlatformMemberJobs(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<ExternalAccessTokenJob>("clean-expired-access-token",
            x => x.CleanExpiredAccessTokenAsync(),
            Cron.Hourly(1));

        recurringJobManager.AddOrUpdate<AutoResetUserPasswordJob>("auto-reset-default-user-password",
            x => x.ResetDefaultUserPasswordAsync(),
            Cron.Minutely());

        recurringJobManager.AddOrUpdate<EnsureSuperUserCreatedJob>("ensure-super-user-created",
            x => x.EnsureSuperUserCreatedAsync(), Cron.Hourly());
    }
}