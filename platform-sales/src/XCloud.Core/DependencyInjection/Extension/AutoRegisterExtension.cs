using System.Reflection;
using FluentAssertions;
using Volo.Abp.DependencyInjection;
using XCloud.Core.DependencyInjection.Exceptions;
using XCloud.Core.Helper;

namespace XCloud.Core.DependencyInjection.Extension;

public static class AutoRegisterExtension
{
    /// <summary>
    /// 无意义的接口，不要注册
    /// </summary>
    public static readonly IReadOnlyCollection<Type> ignore_interfaces = new List<Type>{
        typeof(IDisposable),
        typeof(IDbTableFinder),
        typeof(IFinder),
        typeof(IAutoRegistered),
        typeof(ISingleInstance),
        typeof(IScopedInstance),
        typeof(ITransientDependency),
        typeof(IScopedDependency),
        typeof(ISingletonDependency)
    }.AsReadOnly();

    /// <summary>
    /// 可以注入的接口
    /// </summary>
    public static IEnumerable<Type> GetAllRegableInterfaces(this Type t)
    {
        var res = t.GetInterfaces()
            .Except(ignore_interfaces)
            .ToArray();
        return res;
    }

    static ServiceLifetime GetLifeTime(Type t)
    {
        if (t.IsSingleInstance())
        {
            return ServiceLifetime.Singleton;
        }
        else if (t.IsScopedInstance())
        {
            return ServiceLifetime.Scoped;
        }
        else if (t.IsTransitInstance())
        {
            return ServiceLifetime.Transient;
        }
        else
        {
            return ServiceLifetime.Transient;
        }
    }

    static IEnumerable<KeyValuePair<Type, Type>> GetServicesForRegister(Type implementType)
    {
        yield return new KeyValuePair<Type, Type>(implementType, implementType);

        var serviceTypes = GetAllRegableInterfaces(implementType).ToArray();
        foreach (var service in serviceTypes)
        {
            yield return new KeyValuePair<Type, Type>(service, implementType);
        }
    }

    /// <summary>
    /// 自动扫描可以注册的类
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="search_in_assembly"></param>
    /// <returns></returns>
    public static IServiceCollection AutoRegister(this IServiceCollection collection, Assembly[] search_in_assembly)
    {
        search_in_assembly.Should().NotBeNullOrEmpty();

        var allTypes = search_in_assembly.GetAllTypes()
            .Where(x => x.IsNormalPublicClass())
            .Where(x => x.IsAssignableTo_<IAutoRegistered>())
            .ToArray();

        foreach (var implementType in allTypes)
        {
            var serviceConfig = implementType.GetCustomAttributes_<AutoRegisterdConfigurationAttribute>().FirstOrDefault();
            if (serviceConfig == null)
            {
                serviceConfig = new AutoRegisterdConfigurationAttribute()
                {
                    Ignore = false,
                    TryAdd = false,
                    Replace = false,
                };
            }

            if (serviceConfig.Ignore)
            {
                continue;
            }

            var lifeTime = GetLifeTime(implementType);

            var serviceTypes = GetServicesForRegister(implementType).ToArray();

            foreach (var kv in serviceTypes)
            {
                var descriptor = new ServiceDescriptor(
                    serviceType: kv.Key,
                    implementationType: kv.Value,
                    lifetime: lifeTime);

                if (serviceConfig.Replace)
                {
                    collection.RemoveAll(descriptor.ServiceType);
                }

                //add service
                if (serviceConfig.TryAdd)
                {
                    collection.TryAdd(descriptor);
                }
                else
                {
                    collection.Add(descriptor);
                }
            }
        }

        return collection;
    }

    /// <summary>
    /// 检查是否有不允许重复注册的service，如果有就抛出异常
    /// </summary>
    public static void ThrowWhenMultipleImplement(this IServiceCollection collection)
    {
        var items = collection.Where(x => x.ServiceType.AvoidMultipleImplement())
            .GroupBy(x => x.ServiceType)
            .Select(x => new { x.Key, Impl = x.ToArray() });

        var res = items.Where(x => x.Impl.Length > 1).ToArray();

        if (res.Any())
        {
            throw new RepeatRegException("存在不被允许的重复注册")
            {
                RepeatItems = res
            };
        }
    }
}