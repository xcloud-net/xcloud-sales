using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace XCloud.Core.DependencyInjection.AutofacProvider;

/// <summary>
/// IOC容器
/// https://autofac.org/
/// http://autofac.readthedocs.io/en/latest/getting-started/index.html
/// </summary>
public static class AutofacExtension
{
    public static IServiceProvider AsServiceProvider(this IContainer context) =>
        new AutofacServiceProvider(context);

    /// <summary>
    /// 找出所有实例
    /// </summary>
    public static T[] ResolveAll<T>(this ILifetimeScope scope, string name = null)
    {
        return scope.Resolve_<IEnumerable<T>>().ToArray();
    }

    /// <summary>
    /// 创建实例
    /// </summary>
    public static T Resolve_<T>(this ILifetimeScope scope, string name = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return scope.Resolve<T>();
        }
        else
        {
            return scope.ResolveNamed<T>(name);
        }
    }

    public static T ResolveKeyed_<T>(this ILifetimeScope scope, object serviceKey) =>
        scope.ResolveKeyed<T>(serviceKey);

    public static T ResolveOptionalKeyed_<T>(this ILifetimeScope scope, object serviceKey)
        where T : class =>
        scope.ResolveOptionalKeyed<T>(serviceKey);

    /// <summary>
    /// 没有注册就返回null
    /// </summary>
    public static T ResolveOptional_<T>(this ILifetimeScope scope, string name = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return scope.ResolveOptional<T>();
        }
        else
        {
            return scope.ResolveOptionalNamed<T>(name);
        }
    }
}