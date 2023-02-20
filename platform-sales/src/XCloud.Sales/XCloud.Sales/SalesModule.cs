global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Volo.Abp;
global using Volo.Abp.Auditing;
global using Volo.Abp.DependencyInjection;
global using Volo.Abp.Domain.Entities;
global using Volo.Abp.Domain.Repositories;
global using Volo.Abp.Uow;
global using XCloud.Core.Cache;
global using XCloud.Core.DataSerializer;
global using XCloud.Core.Extension;
global using XCloud.Core.IdGenerator;
global using XCloud.Sales.Services;
using Hangfire;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.FluentValidation;
using Volo.Abp.Http.Client;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using XCloud.Application;
using XCloud.AspNetMvc;
using XCloud.AspNetMvc.RequestCache;
using XCloud.Core.Cache.RequestCache;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Job;
using XCloud.MessageBus;
using XCloud.Redis;
using XCloud.Platform.Connection.WeChat;
using XCloud.Platform.Core.Job;
using XCloud.Platform.Data.EntityFrameworkCore;
using XCloud.Platform.Framework;
using XCloud.Sales.Configuration;
using XCloud.Sales.Core;
using XCloud.Sales.Data.Database;
using XCloud.Sales.Data.DataSeeder;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales;

[DependsOn(
    typeof(AbpLocalizationModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpFluentValidationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpHttpClientModule),
    typeof(AspNetMvcModule),
    typeof(JobModule),
    typeof(RedisModule),
    typeof(MessageBusModule),
    typeof(BaseApplicationServiceModule),
    typeof(MySqlDatabaseModule),
    //wechat
    typeof(WechatModule),
    typeof(PlatformFrameworkModule)
)]
public class SalesModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigSalesLocalization();
        context.AddDbContext();

        Configure<AbpAutoMapperOptions>(option => option.AddMaps<SalesModule>(validate: false));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.RemoveAll<ISpecCombinationParser>();
        context.Services.AddTransient<ISpecCombinationParser, SpecCombinationParser>();
        //request data holder
        context.Services.RemoveAll<IRequestCacheProvider>();
        context.Services.AddScoped<IRequestCacheProvider, HttpContextItemRequestCache>();

        this.Configure<SalesJobOption>(option => { option.AutoStartJob = true; });
        this.Configure<PlatformJobOption>(option =>
        {
            //close platform jobs manually
            option.AutoStartJob = false;
        });
        this.Configure<PlatformEfCoreOption>(option =>
        {
            option.AutoCreateDatabase = false;
        });
        this.Configure<SalesEfCoreOption>(option =>
        {
            option.AutoCreateDatabase = false;
        });
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        Task.Run(async () => await app.TryCreateSalesDatabase()).Wait();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        using var s = app.ApplicationServices.CreateScope();
        if (s.ServiceProvider.AutoStartSalesJob())
        {
            s.ServiceProvider.GetRequiredService<IRecurringJobManager>().StartSalesJobs();
            s.ServiceProvider.GetRequiredService<IBackgroundJobClient>()
                .Schedule<SalesDataSeederExecutor>(
                    methodCall: x => x.Execute(),
                    delay: TimeSpan.FromSeconds(10));
        }
    }
}