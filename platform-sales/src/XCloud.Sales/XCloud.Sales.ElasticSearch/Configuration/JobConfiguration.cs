using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Sales.ElasticSearch.Job;

namespace XCloud.Sales.ElasticSearch.Configuration;

public static class JobConfiguration
{
    public static void ConfigElasticSearchJobs(this IServiceProvider serviceProvider)
    {
        var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

        jobManager.AddOrUpdate<EnsureIndexCreatedJob>(
            "ensure-es-index-created",
            x => x.EnsureIndexCreatedAsync(),
            Cron.Minutely());
    }
}