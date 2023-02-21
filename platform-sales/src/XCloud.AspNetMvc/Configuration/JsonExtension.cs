using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Volo.Abp.Json;
using Volo.Abp.Json.Newtonsoft;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Modularity;
using XCloud.AspNetMvc.Json;
using XCloud.Core.DataSerializer;
using XCloud.Core.DataSerializer.NewtonsoftJson;

namespace XCloud.AspNetMvc.Configuration;

public static class JsonExtension
{
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
}