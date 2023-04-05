using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using XCloud.AspNetMvc.Builder;
using XCloud.AspNetMvc.Configuration;
using XCloud.AspNetMvc.Middleware;
using XCloud.AspNetMvc.Swagger;
using XCloud.Core.Cache.RequestCache;
using XCloud.Core.Extension;
using XCloud.Logging;
using XCloud.AspNetMvc.RequestCache;
using XCloud.Core.Configuration.Builder;
using XCloud.Logging.Serilog;

namespace XCloud.AspNetMvc;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(LoggingModule),
    typeof(AbpSwashbuckleModule)
)]
public class AspNetMvcModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        //添加mvc组件
        context.Services.AddRouting();

        context.Services.AddMvcThenAddSingleton();

        //解决中文被编码
        context.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

        //http context上下文
        context.Services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .RemoveAll<IHttpContextAccessor>()
            .AddHttpContextAccessor();

        context.Services.AddScoped<IRequestCacheProvider, AsyncLocalRequestCache>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var builder = context.Services.GetRequiredXCloudBuilder();

        builder.TryAddSwagger();

        context.ConfigMvcDependency();
        context.ConfigJsonProvider();
        context.ConfigDynamicApi();
        context.ConfigModelValidation();
        
        this.Configure<AbpAntiForgeryOptions>(option => option.AutoValidate = false);
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        //request data holder
        context.Services.RemoveAll<IRequestCacheProvider>();
        context.Services.AddScoped<IRequestCacheProvider, HttpContextItemRequestCache>();
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var pipeline = context.CreateMvcPipelineBuilder();

        pipeline.App.UseRouting();
        pipeline.App.UseCorrelationId();
        pipeline.App.UseSerilogAspNetCoreEnricher();
        
        pipeline.TryEnableSwagger();

        if (pipeline.Environment.IsDevelopment())
        {
            pipeline.App.UseDeveloperExceptionPage();
            pipeline.UseDevelopmentInformation();
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        //
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        using var s = app.ApplicationServices.CreateScope();
        var builder = s.ServiceProvider.GetRequiredService<IXCloudBuilder>();
        var logger = s.ServiceProvider.ResolveLogger<AspNetMvcModule>();

        builder.ExtraProperties.Clear();
    }
}