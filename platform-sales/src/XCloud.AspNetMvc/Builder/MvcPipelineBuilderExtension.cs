using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using XCloud.Core.Configuration;
using XCloud.Core.Configuration.Builder;
using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.AspNetMvc.Builder;

public static class MvcPipelineBuilderExtension
{
    /// <summary>
    /// 创建请求管道配置器,
    /// pipeline配置先后顺序至关重要
    /// </summary>
    public static MvcPipelineBuilder CreateMvcPipelineBuilder(this ApplicationInitializationContext context)
    {
        var res = new MvcPipelineBuilder(context);
        res.App.ApplicationServices.SetAsRootServiceProvider();

        return res;
    }

    internal static IXCloudBuilder AddFileBasedDataProtection(this IXCloudBuilder builder)
    {
        if (builder.GetObject<IDataProtectionBuilder>() == null)
        {
            var collection = builder.Services;
            var config = builder.Configuration;
            var env = collection.GetHostingEnvironment();

            var appName = AppConfig.GetAppName(config, builder.EntryAssembly);

            var dataProtectionBuilder = collection
                .AddDataProtection()
                .SetApplicationName(applicationName: appName)
                .AddKeyManagementOptions(option =>
                {
                    option.AutoGenerateKeys = true;
                    option.NewKeyLifetime = TimeSpan.FromDays(1000);
                });

            dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(env.ContentRootPath));

            builder.SetObject(dataProtectionBuilder);
        }

        return builder;
    }
}