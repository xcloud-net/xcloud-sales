using Volo.Abp;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using XCloud.Redis;

namespace XCloud.MessageBus;

[DependsOn(
    typeof(RedisModule),
    //typeof(EasyAbp.Abp.EventBus.Cap.AbpEventBusCapModule),
    typeof(Volo.Abp.EventBus.AbpEventBusModule)
)]
public class MessageBusModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<AbpDistributedEntityEventOptions>(option =>
        {
            option.AutoEventSelectors.Clear();
        });
        context.Services.AddCapMessageProvider();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.UseMessageBusWorker();
    }
}