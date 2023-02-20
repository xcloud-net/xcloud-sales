using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using XCloud.Core.Extension;

namespace XCloud.Database.EntityFrameworkCore.Mapping.PreConfiguration;

public static class IEntityTypePreConfigurationExtension
{
    public static bool IsEntityTypePreConfigurator(this Type x)
    {
        var res = x.IsClass && x.IsPublic && x.IsGenericType && x.GetGenericArguments().Length == 1 &&
                  x.IsAssignableToGeneric_(typeof(IEntityTypePreConfiguration<>));
        return res;
    }

    public static Type[] FindPreConfigurationProviders(this Assembly a)
    {
        var allTypes = a.GetTypes();
        var preConfigurationProviders = allTypes.Where(x => IsEntityTypePreConfigurator(x)).ToArray();
        return preConfigurationProviders;
    }

    public static bool TryMakePreConfiguratorForEntity(this Type configuratorType, Type entityType, out Type entityConfiguratorType)
    {
        IsEntityTypePreConfigurator(configuratorType).Should().BeTrue();

        var genericEntityArg = configuratorType.GetGenericArguments().FirstOrDefault();
        genericEntityArg.Should().NotBeNull();

        var constraints = genericEntityArg.GetGenericParameterConstraints();

        entityConfiguratorType = null;

        foreach (var c in constraints)
        {
            if (!entityType.IsAssignableTo_(c))
            {
                return false;
            }
        }

        entityConfiguratorType = configuratorType.MakeGenericType(entityType);

        return true;
    }
}