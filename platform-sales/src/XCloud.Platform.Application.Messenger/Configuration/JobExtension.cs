using Hangfire;
using XCloud.Platform.Application.Messenger.Job;

namespace XCloud.Platform.Application.Messenger.Configuration;

public static class JobExtension
{
    public static void StartImWorker(this IServiceProvider provider)
    {
        using var s = provider.CreateScope();

        var manager = s.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        manager.AddOrUpdate<CleanExpiredRegistrationJob>("clean-expired-reg", x => x.ExecuteAsync(), Cron.Minutely());
    }
}