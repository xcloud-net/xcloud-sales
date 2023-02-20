using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace XCloud.Core.Helper;

public static class ValidateHelper
{
    /// <summary>
    /// string dict都是list
    /// </summary>
    public static bool IsNotEmptyCollection<T>(IEnumerable<T> list)
    {
        /*
        IEnumerable<char> x = "fasdfas";
        IEnumerable<string> y = new List<string>();
        IEnumerable<KeyValuePair<string, string>> d = new Dictionary<string, string>();
        */

        return list != null && list.Any();
    }

    /// <summary>
    /// 为空
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsEmptyCollection<T>(IEnumerable<T> list) => !IsNotEmptyCollection(list);

    /// <summary>
    /// 判断是否都是非空字符串
    /// </summary>
    public static bool IsAllNotEmpty(params string[] strs)
    {
        if (IsEmptyCollection(strs))
            throw new ArgumentNullException("至少需要一个参数");

        return strs.All(x => !string.IsNullOrWhiteSpace(x));
    }

    /// <summary>
    /// 根据attribute验证model
    /// </summary>
    public static List<string> CheckEntity_<T>(T model) where T : class
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));
        var list = new List<string>();

        //checker
        bool CheckProp(IEnumerable<ValidationAttribute> validators, PropertyInfo p)
        {
            var data = p.GetValue(model);
            foreach (var validator in validators)
            {
                if (!validator.IsValid(data))
                {
                    list.Add(validator.ErrorMessage);
                    return false;
                }
            }
            return true;
        };

        foreach (var prop in model.GetType().GetProperties())
        {
            if (prop.GetCustomAttributes_<NotMappedAttribute>(inherit: false).Any())
                continue;

            //忽略父级的验证属性
            var validators = prop.GetCustomAttributes_<ValidationAttribute>(inherit: false);
            if (!CheckProp(validators, prop))
                continue;
        }

        list = list.WhereNotEmpty().Distinct().ToList();

        return list;
    }
}