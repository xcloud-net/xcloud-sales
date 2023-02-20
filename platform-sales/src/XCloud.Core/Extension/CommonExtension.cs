using FluentAssertions;

using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;
using XCloud.Core.Dto;
using XCloud.Core.Helper;

namespace XCloud.Core.Extension;

public static class CommonExtension
{
    public static bool HasDuplicateItems(this IEnumerable<string> list)
    {
        var res = list.Count() != list.Distinct().Count();
        return res;
    }

    public static Dictionary<string, string> ConfigAsKV(this IConfiguration config)
    {
        var dict = new Dictionary<string, string>();
        foreach (var kv in config.AsEnumerable())
        {
            dict[kv.Key] = kv.Value;
        }

        return dict;
    }

    public static T ExecuteWithRetry_<T>(this Func<T> action, int retry_count, Func<int, TimeSpan> delay = null)
    {
        if (retry_count < 0)
            throw new ArgumentException($"{nameof(retry_count)}");
        delay = delay ?? (i => TimeSpan.Zero);

        var error = 0;
        while (true)
        {
            try
            {
                var res = action.Invoke();
                return res;
            }
            catch
            {
                ++error;
                if (error > retry_count)
                    throw;
                var wait = delay.Invoke(error);
                if (wait.TotalMilliseconds > 0)
                    System.Threading.Thread.Sleep(wait);
            }
        }
    }

    public static void ExecuteWithRetry_<T>(this Action action, int retry_count, Func<int, TimeSpan> delay = null)
    {
        var res = ExecuteWithRetry_(() =>
        {
            action.Invoke();
            return string.Empty;
        }, retry_count: retry_count, delay: delay);
    }

    /// <summary>
    /// 重试
    /// </summary>
    public static async Task<T> ExecuteWithRetryAsync_<T>(this Func<Task<T>> action, int retry_count, Func<int, TimeSpan> delay = null)
    {
        if (retry_count < 0)
            throw new ArgumentException($"{nameof(retry_count)}");
        delay = delay ?? (i => TimeSpan.Zero);

        var error = 0;
        while (true)
        {
            try
            {
                //这里一定要await，不然task内部的异常将无法抓到
                var res = await action.Invoke();
                return res;
            }
            catch
            {
                ++error;
                if (error > retry_count)
                    throw;
                var wait = delay.Invoke(error);
                if (wait.TotalMilliseconds > 0)
                    await Task.Delay(wait);
            }
        }
    }

    /// <summary>
    /// 重试
    /// </summary>
    /// <param name="action"></param>
    /// <param name="retry_count"></param>
    /// <param name="delay"></param>
    public static async Task ExecuteWithRetryAsync_(this Func<Task> action, int retry_count, Func<int, TimeSpan> delay = null)
    {
        var res = await ExecuteWithRetryAsync_(async () =>
        {
            await action.Invoke();
            return string.Empty;
        }, retry_count: retry_count, delay: delay);
    }

    public static ExceptionDescriptor ExtractExceptionDescriptor(this Exception e, int? maxDeep = null)
    {
        e.Should().NotBeNull();

        maxDeep ??= int.MaxValue;

        var findedList = new List<Exception>();

        ExceptionDescriptor CreateExceptionDescriptor(Exception exception, int parentDeep)
        {
            var currentDeep = ++parentDeep;
            if (exception == null || findedList.Contains(exception) || currentDeep > maxDeep.Value)
            {
                return null;
            }
            findedList.Add(exception);

            var descriptor = new ExceptionDescriptor()
            {
                Message = exception.Message,
                Detail = exception.StackTrace,
                Data = exception.Data
            };

            var innerExceptions = new List<ExceptionDescriptor>();

            if (exception is AggregateException ae && ae.InnerExceptions != null)
            {
                var exceptions = ae.InnerExceptions.Select(x => CreateExceptionDescriptor(x, currentDeep)).ToArray();
                innerExceptions.AddRange(exceptions);
            }

            innerExceptions.Add(CreateExceptionDescriptor(exception.InnerException, currentDeep));

            descriptor.InnerExceptions = innerExceptions.WhereNotNull().ToArray();

            return descriptor;
        }

        return CreateExceptionDescriptor(e, 0);
    }

    /// <summary>
    /// 从list中随机取出一个item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ran"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Choice<T>(this Random ran, IList<T> list)
    {
        return ran.ChoiceIndexAndItem(list).item;
    }

    /// <summary>
    /// 随机抽取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ran"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static (int index, T item) ChoiceIndexAndItem<T>(this Random ran, IList<T> list)
    {
        //The maxValue for the upper-bound in the Next() method is exclusive—
        //the range includes minValue, maxValue-1, and all numbers in between.
        var index = ran.RealNext(minValue: 0, maxValue: list.Count - 1);
        return (index, list[index]);
    }

    /// <summary>
    /// 带边界的随机范围
    /// </summary>
    /// <param name="ran"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int RealNext(this Random ran, int maxValue)
    {
        //The maxValue for the upper-bound in the Next() method is exclusive—
        //the range includes minValue, maxValue-1, and all numbers in between.
        return ran.RealNext(minValue: 0, maxValue: maxValue);
    }

    /// <summary>
    /// 带边界的随机范围
    /// </summary>
    /// <param name="ran"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int RealNext(this Random ran, int minValue, int maxValue)
    {
        //The maxValue for the upper-bound in the Next() method is exclusive—
        //the range includes minValue, maxValue-1, and all numbers in between.
        return ran.Next(minValue: minValue, maxValue: maxValue + 1);
    }

    /// <summary>
    /// 随机抽取一个后从list中移除
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ran"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T PopChoice<T>(this Random ran, ref List<T> list)
    {
        var data = ran.ChoiceIndexAndItem(list);
        list.RemoveAt(data.index);
        return data.item;
    }

    /// <summary>
    /// 从list中随机抽取count个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ran"></param>
    /// <param name="list"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<T> Sample<T>(this Random ran, IList<T> list, int count)
    {
        return new int[count].Select(x => ran.Choice(list)).ToList();
    }

    /// <summary>
    /// 随机选取索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ran"></param>
    /// <param name="list"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<int> SampleIndexs<T>(this Random ran, IList<T> list, int count)
    {
        return ran.Sample(Com.Range(list.Count).ToList(), count);
    }

    /// <summary>
    /// 打乱list的顺序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ran"></param>
    /// <param name="list"></param>
    public static void Shuffle<T>(this Random ran, ref List<T> list)
    {
        var data = new List<T>();
        while (list.Any())
        {
            var itm = ran.PopChoice(ref list);
            data.Add(itm);
        }
        list.AddRange(data);
    }

    /// <summary>
    /// 根据权重选择
    /// </summary>
    public static T ChoiceByWeight<T>(this Random ran, IEnumerable<T> source, Func<T, int> selector)
    {
        source.Should().NotBeNullOrEmpty();

        if (source.Count() == 1)
            return source.First();

        if (source.Any(x => selector.Invoke(x) < 1))
            throw new ArgumentException("权重不能小于1");

        var total_weight = source.Sum(x => selector.Invoke(x));
        //这次命中的权重
        var weight = ran.RealNext(maxValue: total_weight);

        var cur = 0;

        foreach (var s in source)
        {
            //单个权重
            var w = selector.Invoke(s);

            var start = cur;
            var end = start + w;

            //假如在此区间
            if (weight >= start && weight <= end)
            {
                return s;
            }

            cur = end;
        }

        throw new ArgumentException("权重取值异常");
    }
}