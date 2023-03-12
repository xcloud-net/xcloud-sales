using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.Platform.Connection.WeChat.Configuration;
using XCloud.Platform.Core.Job;
using XCloud.Platform.Member.Application;

namespace XCloud.Platform.Connection.WeChat;

[DependsOn(new[]
{
    typeof(PlatformMemberModule)
})]
public class WechatModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigWechat();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        if (app.ApplicationServices.AutoStartPlatformJob())
        {
            using var s = app.ApplicationServices.CreateScope();
            s.ServiceProvider.GetRequiredService<IRecurringJobManager>().StartWechatJobs();
        }
    }
}