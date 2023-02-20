using Microsoft.Extensions.Configuration;
using XCloud.Core;

namespace XCloud.Platform.Connection.WeChat.Configuration;

public static class WechatExtension
{
    public static IConfigurationSection GetWxSection(this IConfiguration configuration)
    {
        var section = configuration.GetSection("Wx"); ;

        if (!section.Exists())
        {
            throw new ConfigException("wechat is not configured");
        }

        return section;
    }

    public static WxMpConfig GetWxMpConfig(this IConfiguration configuration)
    {
        var config = new WxMpConfig();

        var section = GetWxSection(configuration).GetSection("MP");

        if (!section.Exists())
        {
            throw new ConfigException("wechat mp is not configured");
        }

        section.Bind(config);

        return config;
    }
}