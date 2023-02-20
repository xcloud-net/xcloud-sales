namespace XCloud.Core.Extension;

/// <summary>
/// StringExtension
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 去除空格
    /// </summary>
    public static string RemoveWhitespace(this string s) => new string(s.Where(x => x != ' ').ToArray());

    /// <summary>
    /// 后面加url的斜杠
    /// </summary>
    public static string EnsureTrailingSlash(this string input)
    {
        if (!input.EndsWith("/"))
        {
            return input + "/";
        }

        return input;
    }

    /// <summary>
    /// 如果不是有效字符串就转换为null
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string EmptyAsNull(this string str)
    {
        return string.IsNullOrWhiteSpace(str?.Trim()) ?
            null :
            str;
    }

}