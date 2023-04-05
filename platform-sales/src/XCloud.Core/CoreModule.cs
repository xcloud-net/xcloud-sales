global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using XCloud.Core.Extension;
using Castle.DynamicProxy;
using Volo.Abp.Application;
using Volo.Abp.Autofac;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Cache;
using XCloud.Core.Configuration;
using XCloud.Core.Configuration.Builder;
using XCloud.Core.DependencyInjection.ServiceWrapper;
using XCloud.Core.Exceptions;
using XCloud.Core.Http.Dynamic;
using XCloud.Core.Json;

namespace XCloud.Core;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpHttpClientModule),
    typeof(AbpDddApplicationModule)
)]
public class CoreModule : AbpModule
{
    private void ConfigureHttpClient(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClient();
        context.Services.AddTransient<IDynamicClientServiceDiscovery, ServiceDiscoveryViaConfiguration>();
    }

    private void ConfigureExceptionHandler(ServiceConfigurationContext context)
    {
        context.Services.AddMyExceptionHandler();
        context.Services.AddMyExceptionDetailContributor<AbpValidationExceptionContributor>();
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistred(ctx =>
        {
            if (ctx.ImplementationType.IsAssignableTo_<IJsonDataSerializer>())
            {
                //ctx.Interceptors.TryAdd<SerializerExceptionInterceptor>();
            }
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();

        //http client
        this.ConfigureHttpClient(context);
        this.ConfigureExceptionHandler(context);

        //动态代理工厂
        context.Services.AddSingleton(new ServiceWrapper<ProxyGenerator>(new ProxyGenerator()));

        //项目配置
        context.Services.AddScoped<AppConfig>();

        //序列化
        context.Services.AddJsonComponent(config);

        //数据缓存
        context.Services.RemoveAll<ICacheProvider>().AddTransient<ICacheProvider, DistributeCacheProvider>();
        context.Services.AddMemoryCache().AddDistributedMemoryCache();

        //时间
        this.Configure<AbpClockOptions>(option => option.Kind = System.DateTimeKind.Utc);
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();

        //上下文
        context.Services.AddScoped(typeof(IWorkContext<>), typeof(DefaultWorkContext<>));
        context.Services.AddScoped(typeof(IWorkContext), typeof(DefaultWorkContext<>).MakeGenericType(builder.EntryModuleType));
    }
}