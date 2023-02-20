using FluentAssertions;
using System.IO;

namespace XCloud.Core.Helper;

/// <summary>
/// 公共方法类
/// </summary>
public static class Com
{
    public static string ConcatUrl(params string[] paths)
    {
        var list = new List<string>();

        foreach (var p in paths)
        {
            if (string.IsNullOrWhiteSpace(p))
                continue;
            list.Add(p.Trim('/'));
        }

        list = list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        return string.Join('/', list);
    }

    public static bool RoutePathEqual(string path1, string path2)
    {
        if (path1 == null || path2 == null)
            return false;

        if (path1 == path2)
            return true;

        var eq = string.Compare(path1.Trim('/'), path2.Trim('/'), ignoreCase: true) == 0;
        return eq;
    }

    public static void Swap(ref int a, ref int b)
    {
        //bool eq = a << 32 == Math.Pow(2, 32) * a;
        a = a ^ b;
        b = a ^ b;
        a = a ^ b;
    }

    /// <summary>
    /// 使用栈找文件
    /// </summary>
    public static void FindFiles(string path, Action<FileInfo> func, Action<int> stack_count_func = null)
    {
        var root = new DirectoryInfo(path);
        root.Exists.Should().BeTrue();

        var stack = new Stack<DirectoryInfo>();
        stack.Push(root);
        DirectoryInfo cur_node = null;
        while (stack.Count > 0)
        {
            stack_count_func?.Invoke(stack.Count);

            cur_node = stack.Pop();
            if (cur_node == null || !cur_node.Exists) { break; }

            var files = cur_node.GetFiles();
            files.ToList().ForEach(x => { func.Invoke(x); });

            var dirs = cur_node.GetDirectories();
            dirs.ToList().ForEach(x => { stack.Push(x); });
        }
    }

    /// <summary>
    /// 监听目录文件变化
    /// </summary>
    public static FileSystemWatcher WatcherFileSystem(string path, Action<object, FileSystemEventArgs> change, string filter = "*.*")
    {
        var watcher = new FileSystemWatcher();

        try
        {
            watcher.Path = path ?? throw new ArgumentNullException(nameof(path));
            watcher.Filter = filter ?? throw new ArgumentNullException(nameof(filter));

            watcher.Changed += (source, e) => change.Invoke(source, e);
            watcher.Created += (source, e) => change.Invoke(source, e);
            watcher.Deleted += (source, e) => change.Invoke(source, e);
            watcher.Renamed += (source, e) => change.Invoke(source, e);

            watcher.NotifyFilter =
                NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                NotifyFilters.Security | NotifyFilters.Size;

            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            return watcher;
        }
        catch (Exception)
        {
            watcher.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 询价单有效期为30分钟，17.30到第二天早上9点停止报价，如果下午17.20发布询价单，其实是第二天早上才过期
    /// 从开始时间当天开始，计算每天能消耗的时间，不断迭代消耗，直到时间被消耗完
    /// </summary>
    public static DateTime GetExpireTime(DateTime createTime, double DurationSeconds, double day_start_hours = 9, double day_end_hours = 17)
    {
        //当天开始消耗时间的开始位置
        var time = createTime;
        var secondsLeft = DurationSeconds;
        while (true)
        {
            var start = time.Date.AddHours(day_start_hours);
            var end = time.Date.AddHours(day_end_hours);
            //今天实际开始计时的开始时间
            var jishikaishi = default(DateTime);
            if (time < start)
            {
                jishikaishi = start;
            }
            else if (time > end)
            {
                jishikaishi = end;
            }
            else
            {
                jishikaishi = time;
            }
            //今天能消耗的时间
            var xiaohaoshijian = (end - jishikaishi).TotalSeconds;
            //能消耗的时间大于等于剩余的时间
            if (xiaohaoshijian >= secondsLeft)
            {
                //返回最终时间
                return jishikaishi.AddSeconds(secondsLeft);
            }
            //减去消耗时间
            secondsLeft -= xiaohaoshijian;
            //今天没能消耗所有时间，计算下一天
            time = time.Date.AddDays(1).AddHours(day_start_hours);
        }
    }

    /// <summary>
    /// 实现python中的range
    /// </summary>
    public static IEnumerable<int> Range(int a, int? b = null, int step = 1)
    {
        if (step <= 0)
        {
            throw new ArgumentException($"{nameof(step)}必须大于0");
        }

        var list = new List<int?>() { a, b }.WhereNotNull().ToList();
        if (list.Count != 2)
            list.Insert(0, 0);

        var start = list[0].Value;
        var end = list[1].Value;

        for (var i = start; i < end; i += step)
        {
            yield return i;
        }
    }

    /// <summary>
    /// 通用数据转换
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="deft"></param>
    /// <returns></returns>
    public static T ChangeType<T>(object obj, T deft = default)
    {
        try
        {
            var o = Convert.ChangeType(obj, typeof(T));
            if (o is T re)
            {
                return re;
            }
            //强转
            return (T)o;
        }
        catch
        {
            return deft;
        }
    }

    public static int GetPagedSkip(int page, int pageSize)
    {
        (page >= 1 && pageSize >= 1).Should().BeTrue("page和size不能小于1");

        var skip = (page - 1) * pageSize;

        return skip;
    }
}