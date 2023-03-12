using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using XCloud.Core.DependencyInjection.ServiceWrapper;
using XCloud.Core.Json.NewtonsoftJson;

namespace XCloud.Core.Json;

public static class SerializerBuilderExtension
{
    internal static IServiceCollection AddJsonComponent(this IServiceCollection services, IConfiguration config)
    {
        var dateFormat = config.GetDateformatOrDefault();
        var jsonSetting = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
            {
                new IsoDateTimeConverter() { DateTimeFormat =dateFormat  },
            },
            DateFormatString = dateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(),
            MaxDepth = 20
        };
        services.AddSingleton(new ServiceWrapper<JsonSerializerSettings>(jsonSetting));
        services.AddTransient<IJsonDataSerializer, NewtonsoftJsonDataSerializer>();

        return services;
    }
}