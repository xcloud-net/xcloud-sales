using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace XCloud.Core.Threading;

public static class TaskExtension
{
    /// <summary>
    /// ManualResetEvent
    /// AutoResetEvent
    /// ManualResetEventSlim
    /// </summary>
    /// <param name="resetEvent"></param>
    /// <param name="span"></param>
    /// <param name="msg"></param>
    public static void WaitOneOrThrow(this ManualResetEvent resetEvent, TimeSpan span, string msg = null)
    {
        if (!resetEvent.WaitOne(span))
            throw new TimeoutException(msg ?? "等待信号量超时");
    }

    /// <summary>https://msdn.microsoft.com/zh-cn/library/hh873178(v=vs.110).aspx </summary>
    public static IAsyncResult AsApm<T>(this Task<T> task, AsyncCallback callback, object state)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        if (task.AsyncState == state)
        {
            if (callback != null)
                task.ContinueWith(t => callback(t), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return task;
        }

        var tcs = new TaskCompletionSource<T>(state);
        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
                tcs.TrySetException(t.Exception.InnerExceptions);
            else if (t.IsCanceled)
                tcs.TrySetCanceled();
            else
                tcs.TrySetResult(t.Result);

            callback?.Invoke(tcs.Task);
        }, TaskScheduler.Default);

        return tcs.Task;
    }

    public static async Task<T> Timeout<T>(this Task<T> task, TimeSpan delay, CancellationToken token)
    {
        if (await Task.WhenAny(task, Task.Delay(delay, token)) == task)
            return await task;

        throw new TimeoutException();
    }

    //https://blogs.msdn.microsoft.com/pfxteam/2011/01/13/await-anything/
    //https://blogs.msdn.microsoft.com/pfxteam/2012/04/12/asyncawait-faq/
    public static TaskAwaiter<int> GetAwaiter(this Process process)
    {
        var tcs = new TaskCompletionSource<int>();

        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);

        if (process.HasExited)
            tcs.TrySetResult(process.ExitCode);

        return tcs.Task.GetAwaiter();
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>TaskAwaiter.</returns>
    public static TaskAwaiter<bool> GetAwaiter(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        if (cancellationToken.IsCancellationRequested)
            tcs.SetResult(true);
        else
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);

        return tcs.Task.GetAwaiter();
    }

    /// <summary>使用信号号方式限制异步方法并发量</summary>
    /// <param name="source">一个值序列，要对该序列调用转换函数。</param>
    /// <param name="selector">应用于每个元素的转换函数。</param>
    /// <param name="maxParallel">最大并行执行量，≤1时为CPU核心数量</param>
}

public class AsyncManualResetEvent
{
    private volatile TaskCompletionSource<bool> _mTcs = new TaskCompletionSource<bool>();

    public Task WaitAsync() => _mTcs.Task;

    public void Set()
    {
        var tcs = _mTcs;
        Task.Factory.StartNew(s => ((TaskCompletionSource<bool>)s).TrySetResult(true),
            tcs, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
        tcs.Task.Wait();
    }

    public void Reset()
    {
        while (true)
        {
            var tcs = _mTcs;
            if (!tcs.Task.IsCompleted ||
#pragma warning disable 420
                Interlocked.CompareExchange(ref _mTcs,
                    new TaskCompletionSource<bool>(), tcs) == tcs)
                return;
#pragma warning restore 420
        }
    }
}