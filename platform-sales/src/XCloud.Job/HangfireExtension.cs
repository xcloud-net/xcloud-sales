using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.AspNetCore;
using Hangfire.Redis;
using Microsoft.Extensions.Logging;
using XCloud.Core;
using XCloud.Redis;

namespace XCloud.Job;

public static class HangfireExtension
{
    internal static void AddHangfireJobProvider(this IServiceCollection services)
    {
        services.AddHangfire((provider, config) =>
        {
            var appConfig = provider.ResolveAppConfig();
            var redis = provider.GetRequiredService<RedisClient>();

            var options = new RedisStorageOptions()
            {
                Db = (int)RedisConsts.JobScheduler,
                Prefix = appConfig.AppName()
            };

            config.UseRedisStorage(nameOrConnectionString: redis.ConnectionString, options: options);

            config.UseLogProvider(new AspNetCoreLogProvider(provider.GetRequiredService<ILoggerFactory>()));
            config.UseActivator(new AspNetCoreJobActivator(provider.GetRequiredService<IServiceScopeFactory>()));
            config.UseFilter(new AutomaticRetryAttribute() { Attempts = 5 });
        });

        services.AddHangfireServer();
    }

    /// <summary>
    /// dashboard ui不会自动提交token，所以必须使用cookie来实现登录。
    /// 方案：
    /// 1.在bearer token的基础上再加一个cookie authentication。
    /// 2.先登录admin拿到access token。
    /// 3.用token请求授权接口，写入临时cookie authentication（10分钟过期）
    /// 4.hangfire使用cookie authentication验证身份
    /// </summary>
    internal static void UseJobWorker(this IApplicationBuilder builder)
    {
        var options = new DashboardOptions()
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        };
        builder.UseHangfireDashboard("/internal/hangfire", options: options);
    }
}