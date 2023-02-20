namespace XCloud.Core.Extension;

public static class ConvertExtension
{
    private static readonly IReadOnlyCollection<string> bool_string_list =
        new List<string>() {
            "1", "true", "yes", "on", "success", "ok",
            true.ToString().ToLower()
        }.AsReadOnly();

    /// <summary>
    /// true是1，false是0
    /// </summary>
    public static int ToBoolInt(this bool data) => data ? 1 : 0;

    /// <summary>
    /// 转换为布尔值
    /// </summary>
    public static bool ToBool(this string data) =>
        bool_string_list.Contains(data.ToLower());
}