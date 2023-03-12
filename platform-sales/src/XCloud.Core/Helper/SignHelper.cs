using System.Text;
using XCloud.Core.Security.Hash;

namespace XCloud.Core.Helper;

public static class SignHelper
{
    /// <summary>
    /// 筛选+排序
    /// </summary>
    public static SortedDictionary<string, string> FilterAndSort(Dictionary<string, string> dict, string sign_key, IComparer<string> comparer)
    {
        Func<KeyValuePair<string, string>, bool> filter = x =>
        {
            if (x.Key == null || x.Key == sign_key || x.Key.Length > 32 || x.Value?.Length > 32)
            {
                return false;
            }
            else
            {
                return true;
            }
        };
        var filtered = dict.Where(x => filter(x)).ToDictionary(x => x.Key, x => x.Value ?? string.Empty);

        return new SortedDictionary<string, string>(filtered, comparer);
    }

    /// <summary>
    /// 计算签名
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public static (string sign, string sign_data) CreateSign(SortedDictionary<string, string> dict, string salt)
    {
        var strdata = dict.ToUrlParam();
        if (!string.IsNullOrWhiteSpace(salt))
        {
            strdata += salt;
        }
        strdata = strdata.ToLower();

        var md5 = MD5.Encrypt(strdata, encoding: Encoding.UTF8).ToUpper();
        return (md5, strdata);
    }
}