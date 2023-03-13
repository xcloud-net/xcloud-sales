using Hangfire;

namespace XCloud.Platform.Application.Common.Service.Messenger.Jobs;

public static class WorkerExtension
{
    [Obsolete("not ready")]
    public static void StartImWorker(this IServiceProvider provider)
    {
        using var s = provider.CreateScope();

        var manager = s.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        manager.AddOrUpdate<CleanExpiredRegistrationJob>("clean-expired-reg", x => x.ExecuteAsync(), Cron.Minutely());
        manager.AddOrUpdate<ServerCleanupJob>("server-cleanup", x => x.ExecuteAsync(), Cron.Minutely());
        manager.AddOrUpdate<ServerPingJob>("server-ping", x => x.ExecuteAsync(), Cron.Minutely());
    }
}