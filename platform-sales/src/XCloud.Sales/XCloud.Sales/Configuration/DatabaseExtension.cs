using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Redis.DistributedLock;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Database;

namespace XCloud.Sales.Configuration;

public static class DatabaseExtension
{
    public static void AddDbContext(this ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();
        context.Services.AddAbpDbContext<ShopDbContext>(option => { option.AddDefaultRepositories(true); });

        context.Services.Configure<AbpDbContextOptions>(contextOption =>
        {
            contextOption.Configure<ShopDbContext>(option =>
            {
                option.UseMySqlProvider(SalesDbConnectionNames.SalesCloud);
            });
        });
        context.Services.AddScoped(typeof(ISalesRepository<>), typeof(SalesEfRepository<>));
        context.Services.Configure<SalesEfCoreOption>(option => option.AutoCreateDatabase = true);
    }

    public static bool AutoCreateSalesDatabase(this IServiceProvider serviceProvider)
    {
        using var s = serviceProvider.CreateScope();

        var option = s.ServiceProvider.GetRequiredService<IOptions<SalesEfCoreOption>>().Value;

        if (option == null)
            throw new ConfigException(nameof(AutoCreateSalesDatabase));
        
        return option.AutoCreateDatabase;
    }
    
    public static async Task TryCreateSalesDatabase(this IApplicationBuilder app)
    {
        using var s = app.ApplicationServices.CreateScope();

        if (s.ServiceProvider.AutoCreateSalesDatabase())
        {
            using var dlock = await s.ServiceProvider.GetRequiredService<RedLockClient>().RedLockFactory
                .CreateLockAsync("try-create-sales-database-lock-key", TimeSpan.FromSeconds(30));

            if (dlock.IsAcquired)
            {
                using var db = s.ServiceProvider.GetRequiredService<ShopDbContext>();

                await db.Database.EnsureCreatedAsync();
            }
            else
            {
                var logger = s.ServiceProvider.GetRequiredService<ILogger<SalesModule>>();
                logger.LogWarning("failed to get dlock to create platform database");
            }
        }
    }
}