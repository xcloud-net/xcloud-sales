global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using System;
global using System.Linq;

global using Volo.Abp;
global using Volo.Abp.Auditing;
global using Volo.Abp.Uow;

global using XCloud.Core.Cache;
global using XCloud.Core.Extension;
using Hangfire;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Job;
using XCloud.Platform.Common.Application;
using XCloud.Platform.Core.Job;
using XCloud.Platform.Member.Application.Configuration;

namespace XCloud.Platform.Member.Application;

[DependsOn(
    typeof(PlatformCommonModule)
)]
public class PlatformMemberModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformMemberModule>(false));
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        
        if (app.ApplicationServices.AutoStartPlatformJob())
        {
            using var s = app.ApplicationServices.CreateScope();
            s.ServiceProvider.GetRequiredService<IRecurringJobManager>().StartPlatformMemberJobs();
        }
    }
}