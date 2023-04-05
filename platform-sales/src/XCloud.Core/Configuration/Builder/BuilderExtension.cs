using System.Reflection;
using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.Core.Configuration.Builder;

public static class BuilderExtension
{
    /// <summary>
    /// 添加builder，重复添加只会返回第一个
    /// </summary>
    public static IXCloudBuilder AddXCloudBuilder<EntryModule>(this IServiceCollection services) where EntryModule : AbpModule
    {
        var builder = GetXCloudBuilderOrDefault(services);

        if (builder == null)
        {
            builder = new XCloudBuilder<EntryModule>(services);

            services.AddSingleton<IXCloudBuilder>(builder);

            services.AddSingleton(new EntryModuleWrapper(typeof(EntryModule)));

            foreach (var ass in builder.AllModuleAssemblies)
            {
                builder.Services.AutoRegister(new Assembly[] { ass });
            }
        }

        return builder;
    }

    public static IXCloudBuilder GetXCloudBuilderOrDefault(this IServiceCollection services)
    {
        var res = services.GetSingletonInstanceOrNull<IXCloudBuilder>();
        return res;
    }

    public static IXCloudBuilder GetRequiredXCloudBuilder(this IServiceCollection services)
    {
        var res = GetXCloudBuilderOrDefault(services);
        
        if (res == null)
            throw new AbpException("请先注册xcloud builder");
        
        return res;
    }

    static string __key__<T>()
    {
        var t = typeof(T);

        var res = $"object-{t.Namespace}-{t.Name}-from-{t.Assembly.FullName}";

        res = res.Replace(' '.ToString(), string.Empty).Trim();

        return res;
    }

    /// <summary>
    /// 保存到builder
    /// </summary>
    public static IXCloudBuilder SetObject<T>(this IXCloudBuilder builder, T obj)
    {
        var key = __key__<T>();

        builder.ExtraProperties[key] = obj;
        return builder;
    }

    /// <summary>
    /// 从builder中获取对象
    /// </summary>
    public static T GetObject<T>(this IXCloudBuilder builder)
    {
        var key = __key__<T>();

        var obj = builder.ExtraProperties[key];

        /*
         if (obj != null && obj.GetType().IsAssignableTo_<T>())
        {
            var data = (T)obj;
            return data;
        }
         */

        if (obj != null && obj is T res)
        {
            return res;
        }

        return default;
    }
}