using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.Redis;

namespace XCloud.Job;

[DependsOn(
    typeof(RedisModule)
)]
public class JobModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHangfireJobProvider();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.UseJobWorker();
    }
}