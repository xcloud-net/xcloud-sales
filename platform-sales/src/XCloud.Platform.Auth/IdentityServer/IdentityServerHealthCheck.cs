using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Auth.Authentication;

namespace XCloud.Platform.Auth.IdentityServer;

public class IdentityServerHealthCheck : IHealthCheck, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;
    public IdentityServerHealthCheck(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var s = this._serviceProvider.CreateScope();

        try
        {
            var httpClientFacotry = s.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var config = s.ServiceProvider.GetRequiredService<IConfiguration>();

            var httpClient = httpClientFacotry.CreateClient(nameof(IdentityServerHealthCheck));

            var disco = await httpClient.GetIdentityServerDiscoveryDocuments(config);
            disco.IsError.Should().BeFalse("can not get identity server disco");

            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            s.ServiceProvider.GetRequiredService<ILogger<IdentityServerHealthCheck>>().LogError(message: e.Message, exception: e);
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }
}