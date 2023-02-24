using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.Core;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Data.EntityFrameworkCore.Database;
using XCloud.Platform.Shared;
using XCloud.Redis.DistributedLock;

namespace XCloud.Platform.Data.EntityFrameworkCore.Configuration;

public static class DatabaseExtension
{
    public static async Task TryCreatePlatformDatabase(this IApplicationBuilder app)
    {
        using var s = app.ApplicationServices.CreateScope();

        if (s.ServiceProvider.AutoCreatePlatformDatabase())
        {
            using var dlock = await s.ServiceProvider.GetRequiredService<RedLockClient>().RedLockFactory
                .CreateLockAsync("try-create-platform-database-lock-key", TimeSpan.FromSeconds(30));

            if (dlock.IsAcquired)
            {
                using var db = s.ServiceProvider.GetRequiredService<PlatformDbContext>();

                await db.Database.EnsureCreatedAsync();
            }
            else
            {
                var logger = s.ServiceProvider.GetRequiredService<ILogger<PlatformDataEntityFrameworkCoreModule>>();
                logger.LogWarning("failed to get dlock to create platform database");
            }
        }
    }

    internal static void ConfigureDataAccess(this ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpDbContextOptions>(option =>
        {
            option.Configure<PlatformDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseMySqlProvider(PlatformDbConnectionNames.Platform);
            });
        });

        //pool size
        //collection.AddDbContextPool<PlatformDbContext>(option => { }, poolSize: 10);

        context.Services.AddAbpDbContext<PlatformDbContext>(builder =>
        {
            //
        });
        context.Services.AddScoped(typeof(IPlatformRepository<>), typeof(PlatformRepository<>));
        context.Services.AddScoped(typeof(IMemberRepository<>), typeof(PlatformRepository<>));
    }
}