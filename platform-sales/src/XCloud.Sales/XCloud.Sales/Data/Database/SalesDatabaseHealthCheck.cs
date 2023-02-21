using System.Threading;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XCloud.Sales.Data.Database;

[ExposeServices(typeof(SalesDatabaseHealthCheck), typeof(IHealthCheck))]
public class SalesDatabaseHealthCheck : IHealthCheck, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public SalesDatabaseHealthCheck(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var s = this._serviceProvider.CreateScope();

        try
        {
            var db = s.ServiceProvider.GetRequiredService<ShopDbContext>();

            using var connection = db.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var data = await connection.ExecuteScalarAsync<int>("select 1;");

            if (data != 1)
                throw new AbpException("sales database query failed");

            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            s.ServiceProvider.GetRequiredService<ILogger<SalesDatabaseHealthCheck>>()
                .LogError(message: e.Message, exception: e);
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }
}