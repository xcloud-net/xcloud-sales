using XCloud.Core.Helper;

namespace XCloud.Core.Extension;

/// <summary>
/// ValidateExtension
/// </summary>
public static class ValidateExtension
{
    /// <summary>
    /// 判断是否满足数据库约束
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="err"></param>
    /// <returns></returns>
    public static bool IsValid<T>(this T model, out string err) where T : class, IDbTableFinder
    {
        err = model.GetValidErrors().FirstOrDefault();
        return string.IsNullOrWhiteSpace(err);
    }

    /// <summary>
    /// 获取验证错误
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    public static List<string> GetValidErrors<T>(this T model) where T : class, IDbTableFinder
    {
        var res = ValidateHelper.CheckEntity_(model);
        return res;
    }
}