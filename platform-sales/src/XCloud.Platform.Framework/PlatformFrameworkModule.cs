using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc;
using XCloud.Core.Configuration.Builder;
using XCloud.Job;
using XCloud.MessageBus;
using XCloud.Platform.Connection.WeChat;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Core.Job;
using XCloud.Platform.Data.EntityFrameworkCore;
using XCloud.Platform.Framework.Configuration;
using XCloud.Platform.Application.Member;
using XCloud.Platform.Application.Messenger;
using XCloud.Redis;

namespace XCloud.Platform.Framework;

[DependsOn(
    typeof(AspNetMvcModule),
    typeof(RedisModule),
    typeof(JobModule),
    typeof(MessageBusModule),
    typeof(WechatModule),
    typeof(PlatformMemberModule),
    typeof(PlatformMessengerModule),
    typeof(PlatformDataEntityFrameworkCoreModule)
)]
public class PlatformFrameworkModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();

        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformFrameworkModule>(false));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<PlatformJobOption>(option =>
        {
            //disable job auto start
            option.AutoStartJob = true;
        });
        context.Services.Configure<PlatformDatabaseOption>(option =>
        {
            //auto create database
            option.AutoCreateDatabase = false;
        });
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        using var s = context.ServiceProvider.CreateScope();

        if (s.ServiceProvider.AutoStartPlatformJob())
        {
            s.ServiceProvider.StartPlatformFrameworkJobs();
        }
    }
}