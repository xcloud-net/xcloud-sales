using System.Text.RegularExpressions;

namespace XCloud.Core.Helper;

/// <summary>
/// 正则表达式公共类
/// </summary>
public class RegexHelper
{
    /// <summary>
    /// 从字符串中找到匹配的字符
    /// </summary>
    /// <param name="str">原始字符</param>
    /// <param name="pattern">正则</param>
    /// <param name="spliter">分隔符</param>
    /// <returns></returns>
    public static List<Match> FindMatchs(string str, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
    {
        var list = new List<Match>();

        var matchs = Regex.Matches(str, pattern, options);
        foreach (Match matcher in matchs)
        {
            if (matcher == null || !matcher.Success) { continue; }
            list.Add(matcher);
        }

        return list;
    }
}