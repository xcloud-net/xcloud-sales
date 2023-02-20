using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nacos.AspNetCore.V2;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace XCloud.Gateway.Nacos;

public static class NacosExtension
{
    public static string NacosConfigSectionName => "nacos";

    public static void TryAddNacosConfigProvider(this IConfigurationBuilder builder, string sectionName = null)
    {
        sectionName ??= NacosConfigSectionName;

        //nacos
        var configBuilder = new ConfigurationBuilder();
        foreach (var originSource in builder.Sources)
        {
            configBuilder.Add(originSource);
        }
        //只对之前的source有效
        //因为nacos section需要读取文件配置，而文件配置可能使用占位符，所以这里新建一个配置
        configBuilder.AddPlaceholderResolver();

        var nacosSection = configBuilder.Build().GetSection(sectionName);

        if (nacosSection != null && nacosSection.Exists())
        {
            builder.AddNacosV2Configuration(nacosSection);
        }
    }

    public static void TryAddNacosServiceDiscovery(this IServiceCollection services, IConfiguration configuration, string sectionName = null)
    {
        sectionName ??= NacosConfigSectionName;

        var section = configuration.GetSection(sectionName);
        if (section != null && section.Exists())
        {
            services.AddNacosAspNet(configuration, sectionName);
        }
    }
}