using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Platform.Connection.WeChat.Settings;

namespace XCloud.Platform.Connection.WeChat.Configuration;

public static class WechatExtension
{
    public static void ConfigWechat(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var wx = configuration.GetWxSection();

        if (!wx.Exists())
            throw new ConfigException("wx config is required");

        context.Services.Configure<WechatMpOption>(wx.GetSection("Mp"));
        context.Services.Configure<WechatOpenOption>(wx.GetSection("Open"));
    }

    private static IConfigurationSection GetWxSection(this IConfiguration configuration)
    {
        var section = configuration.GetSection("Wechat"); ;

        if (!section.Exists())
        {
            throw new ConfigException("wechat is not configured");
        }

        return section;
    }

    [Obsolete]
    public static WechatMpOption GetWxMpConfig(this IConfiguration configuration)
    {
        var config = new WechatMpOption();

        var section = GetWxSection(configuration).GetSection("Mp");

        if (!section.Exists())
        {
            throw new ConfigException("wechat mp is not configured");
        }

        section.Bind(config);

        return config;
    }
}