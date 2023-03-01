global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using XCloud.Sales.Framework;
global using Volo.Abp.Domain.Entities;
global using XCloud.Core.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

using XCloud.AspNetMvc;
using XCloud.AspNetMvc.Builder;
using XCloud.AspNetMvc.Swagger;
using XCloud.Core.Builder;
using XCloud.Redis;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data.Database;
using XCloud.Sales.ElasticSearch;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Stores;

namespace XCloud.Sales.Mall.Api;

[DependsOn(
    typeof(SalesModule),
    //optional services
    //typeof(SalesElasticSearchModule),
    typeof(AspNetMvcModule)
)]
[SwaggerConfiguration(ServiceName, ServiceName)]
public class SalesMallApiModule : AbpModule
{
    public const string ServiceName = "mall";

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistred(c =>
        {
            if (c.ImplementationType.IsAssignableTo_<ControllerBase>())
            {
                c.Interceptors.TryAdd<AuditLogInterceptor>();
            }
        });
        context.Services.AddXCloudBuilder<SalesMallApiModule>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(option => option.AddMaps<SalesMallApiModule>(false));

        Configure<MvcOptions>(options =>
        {
            //options.Filters.Add<GlobalExceptionsFilter>(int.MaxValue);
        });

        var healthCheckBuilder = context.Services.AddHealthChecks();
        healthCheckBuilder.AddCheck<RedisHealthCheck>(nameof(RedisHealthCheck));
        healthCheckBuilder.AddCheck<PlatformServiceHealthCheck>(nameof(PlatformServiceHealthCheck));
        healthCheckBuilder.AddCheck<SalesDatabaseHealthCheck>(nameof(SalesDatabaseHealthCheck));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        var env = context.Services.GetHostingEnvironment();
        if (env.IsDevelopment() || env.IsStaging() || true)
        {
            context.Services.RemoveAll<ICurrentStoreSelector>();
            context.Services.AddTransient<ICurrentStoreSelector, DevCurrentStoreSelector>();
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var pipeline = context.CreateMvcPipelineBuilder();

        //multiple language
        pipeline.App.UseAbpRequestLocalization();

        pipeline.App.UseMiddleware<StoreAuthMiddleware>();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/internal/healthz");
            endpoints.MapDefaultControllerRoute();
        });
    }
}