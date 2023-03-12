using DotNetCore.CAP;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Configuration;
using XCloud.Core.Configuration.Builder;
using XCloud.Redis;

namespace XCloud.MessageBus;

public static class MessageBusExtension
{
    internal static void AddCapMessageProvider(this IServiceCollection services)
    {
        var builder = services.GetRequiredXCloudBuilder();
        var configuration = builder.Services.GetConfiguration();

        var appName = AppConfig.GetAppName(configuration, builder.EntryAssembly);

        var client = services.GetSingletonInstance<RedisClient>();
        services.AddCap(option =>
        {
            option.UseRedis(connection: client.ConnectionString);
            option.UseInMemoryStorage();

            //isolate
            option.GroupNamePrefix = appName;
            option.TopicNamePrefix = appName;

            option.UseDashboard(x =>
            {
                x.PathMatch = "/internal/cap";
            });
        });
    }

    internal static void UseMessageBusWorker(this IApplicationBuilder builder)
    {
        builder.UseCapDashboard();
    }
}