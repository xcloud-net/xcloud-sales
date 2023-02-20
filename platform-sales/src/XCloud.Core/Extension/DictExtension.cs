using System.Net;

namespace XCloud.Core.Extension;

public static class DictExtension
{
    /// <summary>
    /// 字典变url格式(a=1&b=3)
    /// </summary>
    public static string ToUrlParam(this IDictionary<string, string> dict)
    {
        var d = dict.Where(x => x.Key?.Length > 0).Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value ?? string.Empty)}");
        var res = string.Join("&", d);
        return res;
    }

    /// <summary>
    /// 把一个字典加入另一个字典，重复就覆盖
    /// </summary>
    public static Dictionary<K, V> AddDict<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> data)
    {
        foreach (var kv in data)
        {
            dict[kv.Key] = kv.Value;
        }
        return dict;
    }

    /// <summary>
    /// 获取值
    /// </summary>
    public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dict, K key, V deft = default(V))
    {
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        return deft;
    }
}