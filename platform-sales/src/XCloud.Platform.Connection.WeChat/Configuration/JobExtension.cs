using Hangfire;
using XCloud.Platform.Connection.WeChat.Job;

namespace XCloud.Platform.Connection.WeChat.Configuration;

public static class JobExtension
{
    public static void StartWechatJobs(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<WechatMpJobs>("refresh-mp-access-token", x => x.RefreshClientAccessTokenAsync(),
            Cron.Minutely());
    }
}