using System.ComponentModel;
using System.Reflection;

namespace XCloud.Core.Extension;

public static class EnumExtension
{
    /// <summary>
    /// 用来获取枚举成员
    /// </summary>
    public static Dictionary<string, object> GetEnumFieldsValues(this Type t)
    {
        if (!t.IsEnum)
            throw new ArgumentException($"{t.FullName}must be enum");

        string GetFieldName(MemberInfo m)
        {
            var res = m.GetCustomAttribute<DescriptionAttribute>()?.Description ?? m.Name;
            return res;
        }

        var fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);

        return fields.ToDict(x => GetFieldName(x), x => x.GetValue(null));
    }
}