using System.Reflection;
using FluentAssertions;

namespace XCloud.Core.Http.Dynamic.Definition;

public static class ServiceDefinitionHelper
{
    public static bool IsParameterSame(ParameterInfo a, ParameterInfo b)
    {
        if (a == null || b == null)
        {
            return false;
        }
        if (a.Name != b.Name || a.ParameterType != b.ParameterType || a.Position != b.Position)
        {
            return false;
        }

        return true;
    }

    public static bool IsMethodParameterSame(MethodInfo impl, MethodInfo define)
    {
        var impl_parameters = impl.GetParameters().OrderBy(x => x.Position).ToArray();
        var define_parameters = define.GetParameters().OrderBy(x => x.Position).ToArray();
        if (impl_parameters.Length != define_parameters.Length)
        {
            return false;
        }

        for (var i = 0; i < impl_parameters.Length; ++i)
        {
            var a = impl_parameters[i];
            var b = define_parameters[i];
            if (!IsParameterSame(a, b))
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsImplementRelation(MethodInfo impl, MethodInfo define)
    {
        return impl.Name == define.Name &&
               impl.IsGenericMethod == define.IsGenericMethod &&
               impl.IsPublic == define.IsPublic &&
               IsMethodParameterSame(impl, define);
    }

    public static Type GetServiceContractInterfaceOrNull(Type type)
    {
        type.Should().NotBeNull();
        type.IsNormalPublicClass().Should().BeTrue();

        var contracts = type.GetInterfaces();
        var res = contracts.FirstOrDefault(x => x.GetCustomAttributes_<DynamicHttpClientAttribute>().Any());
        return res;
    }
}