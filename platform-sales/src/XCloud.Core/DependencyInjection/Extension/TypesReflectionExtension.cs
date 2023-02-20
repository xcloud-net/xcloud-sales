using XCloud.Core.DependencyInjection.Attributes;

namespace XCloud.Core.DependencyInjection.Extension;

public static class TypesReflectionExtension
{
    /// <summary>
    /// 避免一个抽象有多个实现
    /// </summary>
    /// <returns><c>true</c>, if repeat was checked, <c>false</c> otherwise.</returns>
    /// <param name="t">T.</param>
    public static bool AvoidMultipleImplement(this Type t)
    {
        var res = t.GetCustomAttributes_<AvoidMultipleImplementAttribute>().Any();
        return res;
    }

    /// <summary>
    /// 是否注册为单例
    /// </summary>
    public static bool IsSingleInstance(this Type t)
    {
        var res = t.IsAssignableTo_<ISingleInstance>();
        return res;
    }

    /// <summary>
    /// scope
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsScopedInstance(this Type t)
    {
        var res = t.IsAssignableTo_<IScopedInstance>();
        return res;
    }

    public static bool IsTransitInstance(this Type t)
    {
        var res = t.IsAssignableTo_<ITransitInstance>();
        return res;
    }
}