using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Volo.Abp.Timing;

namespace XCloud.Sales.Clients.Platform;

/// <summary>
/// 404 means service is service lived
/// </summary>
public class PlatformServiceHealthCheck : IHealthCheck, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public PlatformServiceHealthCheck(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var s = _serviceProvider.CreateScope();

        try
        {
            var clock = s.ServiceProvider.GetRequiredService<IClock>();
            var client = s.ServiceProvider.GetRequiredService<IPlatformClientFactory>();
            var httpClient = await client.CreateClientAsync();

            using var res = await httpClient.GetAsync($"/notfound-url-{clock.Now.Ticks}",
                cancellationToken: cancellationToken);

            if (res.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                return HealthCheckResult.Unhealthy("responsed http code should be 404");
            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            var logger = s.ServiceProvider.GetRequiredService<ILogger<PlatformServiceHealthCheck>>();
            logger.LogError(message: e.Message, exception: e);
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }
}