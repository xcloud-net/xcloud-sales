using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace XCloud.Core.Threading;

/// <summary>
/// await anything
/// https://blogs.msdn.microsoft.com/pfxteam/2011/01/13/await-anything/
/// https://blogs.msdn.microsoft.com/pfxteam/2012/04/12/asyncawait-faq/
/// </summary>
public static class AwaitExtension
{
    public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan) =>
        Task.Delay(timeSpan).GetAwaiter();

    static async void test()
    {
        await TimeSpan.FromSeconds(100);
    }

    public static TaskAwaiter<int> GetAwaiter(this Process process)
    {
        var tcs = new TaskCompletionSource<int>();

        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);

        if (process.HasExited)
            tcs.TrySetResult(process.ExitCode);

        return tcs.Task.GetAwaiter();
    }

    public static TaskAwaiter<bool> GetAwaiter(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        if (cancellationToken.IsCancellationRequested)
            tcs.SetResult(true);
        else
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);

        return tcs.Task.GetAwaiter();
    }
}