using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace XCloud.Redis;

public class RedisHealthCheck : IHealthCheck, ITransientDependency
{
    private readonly IServiceProvider serviceProvider;
    public RedisHealthCheck(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var s = this.serviceProvider.CreateScope();

        try
        {
            var client = s.ServiceProvider.GetRequiredService<RedisClient>();

            await client.Connection.GetDatabase().KeyExistsAsync(nameof(RedisHealthCheck));

            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            s.ServiceProvider.GetRequiredService<ILogger<RedisHealthCheck>>().LogError(message: e.Message, exception: e);
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }
}