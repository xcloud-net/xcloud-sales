using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc.DynamicApi;
using XCloud.AspNetMvc.ExceptionHandling;
using XCloud.AspNetMvc.Filters;
using XCloud.AspNetMvc.ModelBinder;
using XCloud.AspNetMvc.ModelValidation;
using XCloud.Logging;

namespace XCloud.AspNetMvc.Configuration;

public static class AspNetMvcExtension
{
    public static void ConfigModelValidation(this ServiceConfigurationContext context)
    {
        context.Services.RemoveAll<IObjectModelValidator>()
            .AddSingleton<IObjectModelValidator, NullObjectModelValidator>();

        var mvcBuilder = context.Services.GetSingletonInstance<IMvcBuilder>();
    }
    
    public static void ConfigDynamicApi(this ServiceConfigurationContext context)
    {
        var builder = context.Services.GetMvcBuilder();

        builder.PartManager.FeatureProviders.Add(new DynamicApiControllerFeatureProvider());

        builder.Services.AddTransient<IApplicationModelProvider, DynamicApiApplicationModelProvider>();

        context.Services.Configure<MvcOptions>(option => { option.Conventions.Add(new DynamicApiApplicationModelConvention()); });
    }
    
    public static void ConfigMvcDependency(this ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();
        var env = context.Services.GetHostingEnvironment();

        context.Services.Configure<MvcOptions>(option =>
        {
            //option.ValueProviderFactories.Add(new QueryStringValueProviderFactory());
            option.ModelBinderProviders.Insert(0, new MyModelBinderProvider());
            option.Filters.AddService<NormalizeResponseFilter>();
            option.AddExceptionHandler();
        });

        //异常捕捉
        context.Services.Configure<AbpExceptionHandlingOptions>(option =>
        {
            option.SendExceptionsDetailsToClients = env.IsDevelopment() || env.IsStaging() || config.IsDebug();
#if DEBUG
            option.SendExceptionsDetailsToClients = true;
#endif
        });
    }
    
    public static bool IsInternalEnvironment(this IConfiguration config)
    {
        var isInternal = config["app:config:internal"] == "true";
        return isInternal;
    }

    public static IMvcBuilder AddMvcThenAddSingleton(this IServiceCollection services)
    {
        var builder = services.AddMvc();

        services.AddSingleton<IMvcBuilder>(builder);

        return builder;
    }

    public static IMvcBuilder GetMvcBuilder(this IServiceCollection services)
    {
        var builder = services.GetSingletonInstance<IMvcBuilder>();
        return builder;
    }
}