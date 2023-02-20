using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Platform.Framework.DataSeeder;

namespace XCloud.Platform.Framework.Configuration;

public static class JobExtension
{
    public static void StartPlatformFrameworkJobs(this IServiceProvider serviceProvider)
    {
        using var s = serviceProvider.CreateScope();
        
        s.ServiceProvider.GetRequiredService<IBackgroundJobClient>()
            .Schedule<PlatformDataSeederExecutor>(
                methodCall: x => x.Execute(),
                delay: TimeSpan.FromSeconds(10));
    }
}