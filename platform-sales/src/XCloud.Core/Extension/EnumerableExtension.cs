using FluentAssertions;

using System.Collections;
using Volo.Abp;
using XCloud.Core.Helper;

namespace XCloud.Core.Extension;

public static class EnumerableExtension
{
    public static IEnumerable<TSource> InBy<TSource, TKey>(this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer = null)
    {
        if (first == null || second == null || keySelector == null)
            throw new ArgumentNullException(nameof(NotInBy));

        var secondKeys = second.Select(keySelector).ToArray();

        Func<TKey, bool> ContainsBySecond;

        if (comparer != null)
            ContainsBySecond = x => secondKeys.Contains(x, comparer);
        else
            ContainsBySecond = x => secondKeys.Contains(x);

        foreach (var m in first)
        {
            var key = keySelector.Invoke(m);

            if (ContainsBySecond(key))
                yield return m;
        }
    }

    /// <summary>
    /// fix of except by
    /// </summary>
    public static IEnumerable<TSource> NotInBy<TSource, TKey>(this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer = null)
    {
        if (first == null || second == null || keySelector == null)
            throw new ArgumentNullException(nameof(NotInBy));

        var secondKeys = second.Select(keySelector).ToArray();

        Func<TKey, bool> ContainsBySecond;

        if (comparer != null)
            ContainsBySecond = x => secondKeys.Contains(x, comparer);
        else
            ContainsBySecond = x => secondKeys.Contains(x);

        foreach (var m in first)
        {
            var key = keySelector.Invoke(m);

            if (ContainsBySecond(key))
                continue;

            yield return m;
        }
    }

    public static bool IsEmtpy<T>(this IEnumerable<T> list) => !list.Any();

    public static IEnumerable<TResult> ItemOfType<TResult>(this IEnumerable source)
    {
        foreach (var m in source)
        {
            if (m is TResult d)
            {
                yield return d;
            }
        }
    }

    /// <summary>
    /// 不修改list，返回新list
    /// </summary>
    public static IEnumerable<T> AppendManyItems<T>(this IEnumerable<T> list, params T[] data)
    {
        foreach (var m in list)
        {
            yield return m;
        }

        foreach (var m in data)
        {
            yield return m;
        }
    }

    /// <summary>
    /// 修改list然后返回
    /// </summary>
    public static List<T> AddManyItems<T>(this List<T> list, params T[] data)
    {
        if (ValidateHelper.IsNotEmptyCollection(data))
        {
            list.AddRange(data);
        }
        return list;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
    {
        var res = list.Where(x => x != null).ToArray();
        return res;
    }

    public static IEnumerable<string> WhereNotEmpty(this IEnumerable<string> list)
    {
        var res = list.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        return res;
    }

    /// <summary>
    /// 移除
    /// </summary>
    public static void RemoveWhere<T>(this List<T> list, Func<T, bool> where)
    {
        for (var i = list.Count - 1; i >= 0; --i)
        {
            var item = list[i];
            if (where.Invoke(item))
            {
                list.Remove(item);
            }
        }
    }

    /// <summary>
    /// 在list中添加item，遇到重复就抛异常
    /// </summary>
    public static List<string> AddOnceOrThrow(this List<string> list, string flag, string error_msg = null)
    {
        if (list.Contains(flag))
        {
            throw new UserFriendlyException(error_msg ?? $"{flag}已经存在");
        }

        list.Add(flag);
        return list;
    }

    /// <summary>
    /// 反转
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> ReverseItems<T>(this IEnumerable<T> list)
    {
        var data = list.ToList();
        if (data.Count < 2)
            return data;

        for (int i = 0; i < data.Count / 2; ++i)
        {
            var temp = data[i];
            var second_index = data.Count - 1 - i;

            data[i] = data[second_index];
            data[second_index] = temp;
        }

        return data;
    }

    /// <summary>
    /// 不会因为重复key报错，后面的key会覆盖前面的key
    /// </summary>
    public static Dictionary<K, V> ToDict<T, K, V>(this IEnumerable<T> list, Func<T, K> key_selector, Func<T, V> value_selector)
    {
        var dict = new Dictionary<K, V>();

        foreach (var m in list)
        {
            var key = key_selector.Invoke(m);
            var value = value_selector.Invoke(m);

            dict[key] = value;
        }

        return dict;
    }

    /// <summary>
    /// 执行Reduce（逻辑和python一样），集合至少2个item
    /// </summary>
    public static T Reduce<T>(this IList<T> list, Func<T, T, T> func)
    {
        if (list.Count < 2)
            throw new ArgumentException($"item少于2的list无法执行{nameof(Reduce)}操作");
        if (func == null)
            throw new ArgumentNullException($"{nameof(Reduce)}.{nameof(func)}");

        var res = func.Invoke(list[0], list[1]);
        /*
         * not sure list.skip works before sort those data
        foreach (var m in list.Skip(2))
        {
            res = func.Invoke(res, m);
        }*/
        for (var i = 2; i < list.Count; ++i)
        {
            res = func.Invoke(res, list[i]);
        }
        return res;
    }

    /// <summary>
    /// 集合分批处理
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch_<T>(this IEnumerable<T> source, int batchSize)
    {
        source.Should().NotBeNull();
        (batchSize > 0).Should().BeTrue("batch size必须大于0");

        IEnumerable<T> __batch__(IEnumerator<T> enumerator, int size)
        {
            do
            {
                yield return enumerator.Current;
                //you can change the size,because int is not reference type
                //every batch you get new number of batch size
            }
            while (--size > 0 && enumerator.MoveNext());
        }

        using (var enumerator = source.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                yield return __batch__(enumerator, batchSize);
            }
        }
    }
}