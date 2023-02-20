using Microsoft.Extensions.Configuration;
using XCloud.Core.DependencyInjection.ServiceWrapper;

namespace XCloud.Core.DependencyInjection.Extension;

public static class IocContextExtension
{
    /// <summary>
    /// 作为顶级容器
    /// </summary>
    public static IServiceProvider SetAsRootServiceProvider(this IServiceProvider provider)
    {
        IocContext.Instance.SetRootContainer(provider);
        return provider;
    }

    public static IConfiguration ResolveConfiguration(this IServiceProvider provider)
    {
        var res = provider.GetRequiredService<IConfiguration>();
        return res;
    }

    //-------------------------------------------------------------------------

    public static IServiceCollection RemoveByFilter(this IServiceCollection collection, Func<ServiceDescriptor, bool> where)
    {
        var query = collection.AsEnumerable().Where(where);
        var remove_list = query.ToArray();

        //这里最好tolist，防止query中的值被修改
        foreach (var m in remove_list)
        {
            collection.Remove(m);
        }

        return collection;
    }

    /// <summary>
    /// 存在service
    /// </summary>
    public static bool ExistService_(this IServiceCollection collection, Type service)
    {
        var res = collection.Any(x => x.ServiceType == service);
        return res;
    }

    //-----------------------------------------------------------------------------

    public static void DisposeSingleInstanceService(this IServiceProvider provider)
    {
        //dispose single instances
        using var s = provider.CreateScope();
        var logger = s.ServiceProvider.ResolveLoggerFactory().CreateLogger(nameof(DisposeSingleInstanceService));

        //释放
        var components = s.ServiceProvider.GetServices<ISingleInstanceService>().ToArray();
        components = components.OrderBy(x => x.DisposeOrder).ToArray();

        foreach (var com in components)
        {
            try
            {
                //dispose by using syntax
                using (com) { }
            }
            catch (Exception e)
            {
                logger.LogError(message: e.Message, exception: e);
            }
        }
    }
}