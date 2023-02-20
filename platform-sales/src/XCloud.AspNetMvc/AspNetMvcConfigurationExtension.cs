using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc.ExceptionHandling;
using XCloud.AspNetMvc.Filters;
using XCloud.AspNetMvc.ModelBinder;
using XCloud.Logging;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Volo.Abp.Json;
using Volo.Abp.Json.Newtonsoft;
using Volo.Abp.Json.SystemTextJson;
using XCloud.Core.DataSerializer;
using XCloud.Core.DataSerializer.NewtonsoftJson;
using XCloud.AspNetMvc.DynamicApi;
using XCloud.AspNetMvc.ModelValidation;

namespace XCloud.AspNetMvc;

public static class AspNetMvcConfigurationExtension
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
    
    public static void ConfigJsonProvider(this ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();
        var dateFormat = config.GetDateformatOrDefault();

        //system.text.json
        context.Services.Configure<JsonOptions>(option =>
        {
            option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            option.JsonSerializerOptions.PropertyNamingPolicy = null;
            option.JsonSerializerOptions.DictionaryKeyPolicy = null;
            option.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            option.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            option.JsonSerializerOptions.MaxDepth = 30;
        });
        
        //setting abp json
        context.Services.Configure<AbpSystemTextJsonSerializerOptions>(option =>
        {
            option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            option.JsonSerializerOptions.PropertyNamingPolicy = null;
            option.JsonSerializerOptions.DictionaryKeyPolicy = null;
            option.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            option.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            option.JsonSerializerOptions.MaxDepth = 30;
        });

        //newtonsoft.json
        context.Services.Configure<MvcNewtonsoftJsonOptions>(option =>
        {
            option.SerializerSettings.DateFormatString = dateFormat;
            option.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            option.SerializerSettings.MaxDepth = 10;
            option.UseMemberCasing();
        });
        context.Services.Configure<AbpNewtonsoftJsonSerializerOptions>(option => { });
        //abp json settings
        context.Services.Configure<AbpJsonOptions>(option =>
        {
            option.UseHybridSerializer = false;
            option.DefaultDateTimeFormat = dateFormat;
            option.Providers.Remove<AbpSystemTextJsonSerializerProvider>();
        });

        context.Services.RemoveAll<INewtonsoftJsonOptionAccessor>();
        context.Services.AddTransient<INewtonsoftJsonOptionAccessor, MvcNewtonsoftJsonOptionAccessor>();
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