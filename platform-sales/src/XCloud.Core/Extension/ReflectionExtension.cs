using FluentAssertions;

using System.Reflection;
using System.Threading.Tasks;
using XCloud.Core.Helper;

namespace XCloud.Core.Extension;

/// <summary>
/// BaseType是类
/// GetInterfaces是接口
/// IsGenericType是泛型
/// GetGenericTypeDefinition()获取泛型类型比如Consumer《string》
/// </summary>
public static class ReflectionExtension
{
    /// <summary>
    /// 找到依赖的所有程序集
    /// </summary>
    public static IEnumerable<Assembly> FindAllReferencedAssemblies(this Assembly entry, Func<Assembly, bool> filter = null)
    {
        filter ??= x => true;
        var finded = new List<Assembly>();

        var all_ass = AppDomain.CurrentDomain.GetAssemblies();

        Assembly __get_by_name__(AssemblyName name) => all_ass.FirstOrDefault(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(), name));

        IEnumerable<Assembly> __find__(Assembly ass)
        {
            ass.Should().NotBeNull();

            if (!finded.Contains(ass) && filter.Invoke(ass))
            {
                yield return ass;
                finded.Add(ass);
                //找到依赖
                var referenced_ass = ass.GetReferencedAssemblies().Select(x => __get_by_name__(x)).WhereNotNull().ToArray();
                foreach (var m in referenced_ass)
                {
                    foreach (var finded in __find__(m))
                    {
                        yield return finded;
                    }
                }
            }
        }

        var res = __find__(entry);
        return res;
    }

    public static bool IsNullable(this Type t)
    {
        var res = t.IsGenericType_(typeof(Nullable<>));
        return res;
    }

    public static bool IsDatabaseTable(this Type t)
    {
        var res = t.IsNormalPublicClass() && t.IsAssignableTo_<IDbTableFinder>();
        return res;
    }

    /// <summary>
    /// 找到表对象
    /// </summary>
    public static IEnumerable<Type> FindEntity_(this Assembly ass)
    {
        var res = ass.GetTypes().Where(x => IsDatabaseTable(x));
        return res;
    }

    /// <summary>
    /// 获取所有类型
    /// </summary>
    /// <param name="ass"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> ass)
    {
        var res = ass.SelectMany(x => x.GetTypes());
        return res;
    }

    public static bool IsAsyncResultType(this Type t)
    {
        var res = t.IsAssignableTo_<IAsyncResult>();
        return res;
    }

    public static bool IsTask(this Type t)
    {
        var res =
            t == typeof(Task) ||
            t.IsGenericType_(typeof(Task<>));
        return res;
    }

    public static bool IsValueTask(this Type t)
    {
        var res =
            t == typeof(ValueTask) ||
            t.IsGenericType_(typeof(ValueTask<>));
        return res;
    }

    public static Type UnWrapTaskOrNull(this Type t)
    {
        t.Should().NotBeNull();

        if (t.IsGenericType_(typeof(Task<>)))
        {
            var realType = t.GetGenericArguments().FirstOrDefault();
            return realType;
        }
        return null;
    }

    /// <summary>
    /// 判断是否是泛型的子类
    /// </summary>
    public static bool IsAssignableToGeneric_(this Type t, Type generic_type)
    {
        generic_type.IsGenericType.Should().BeTrue("必须是泛型");

        if (generic_type.IsInterface)
        {
            //bug
            var res = t.GetInterfaces().Any(x => x.IsGenericType_(generic_type));
            return res;
        }
        else
        {
            var baseTypes = new List<Type>();

            IEnumerable<Type> FindBaseTypes(Type a)
            {
                var baseType = a.BaseType;

                if (baseType == null || baseType == typeof(object))
                    yield break;

                if (baseTypes.Contains(baseType))
                    yield break;
                baseTypes.Add(baseType);

                yield return baseType;

                foreach (var m in FindBaseTypes(a.BaseType))
                    yield return m;
            }

            var res = new[] { t }.AppendManyItems(FindBaseTypes(t).ToArray()).Any(x => x.IsGenericType_(generic_type));
            return res;
        }
    }

    /// <summary>
    /// 是否可以赋值给
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsAssignableTo_<T>(this Type t)
    {
        var res = t.IsAssignableTo_(typeof(T));
        return res;
    }

    /// <summary>
    /// 是否可以赋值给
    /// </summary>
    /// <param name="t"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAssignableTo_(this Type t, Type type)
    {
        var res = type.IsAssignableFrom(t);
        return res;
    }

    /// <summary>
    /// 不是抽象类，不是接口
    /// </summary>
    public static bool IsNormalPublicClass(this Type t)
    {
        var res = t.IsPublic && t.IsClass && !t.IsAbstract && !t.IsInterface;
        return res;
    }

    /// <summary>
    /// 是指定的泛型
    /// </summary>
    /// <param name="t"></param>
    /// <param name="tt"></param>
    /// <returns></returns>
    public static bool IsGenericType_(this Type t, Type tt)
    {
        tt.IsGenericType.Should().BeTrue("传入参数必须是泛型");

        var res = t.IsGenericType && t.GetGenericTypeDefinition() == tt;
        return res;
    }

    /// <summary>
    /// 获取可以赋值给T的属性
    /// </summary>
    public static IEnumerable<T> GetCustomAttributes_<T>(this MemberInfo prop, bool inherit = true) where T : Attribute
    {
        var attrs = CustomAttributeExtensions.GetCustomAttributes(prop, inherit);
        var res = attrs.Where(x => x.GetType().IsAssignableTo_<T>()).Select(x => (T)x).ToArray();
        return res;
    }

    public static MethodInfo[] GetPrivateMethods(this Type t)
    {
        var res = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        return res;
    }

    public static MethodInfo[] GetPublicStaticMethods(this Type t)
    {
        var res = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
        return res;
    }
}