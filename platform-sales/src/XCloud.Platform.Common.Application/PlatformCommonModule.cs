global using System;
global using System.Linq;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using XCloud.Core.IdGenerator;
global using Volo.Abp;
global using Volo.Abp.Auditing;
using Hangfire;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Platform.Common.Application.Configuration;
using XCloud.Platform.Core;
using XCloud.Platform.Core.Job;

namespace XCloud.Platform.Common.Application;

[DependsOn(
    typeof(PlatformCoreModule)
)]
public class PlatformCommonModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigStorageAll();

        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformCommonModule>(false));
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        if (app.ApplicationServices.AutoStartPlatformJob())
        {
            using var s = app.ApplicationServices.CreateScope();
            s.ServiceProvider.GetRequiredService<IRecurringJobManager>().StartPlatformCommonJobs();
        }
    }
}