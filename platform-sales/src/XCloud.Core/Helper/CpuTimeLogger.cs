using System.Diagnostics;

namespace XCloud.Core.Helper;

/// <summary>
/// 运行计时
/// </summary>
public class CpuTimeLogger : IDisposable
{
    private readonly Stopwatch timer = new Stopwatch();

    public Action<long> OnStop { get; set; }

    public CpuTimeLogger(Action<long> OnStop)
    {
        this.OnStop = OnStop;

        timer.Start();
    }

    public void Dispose()
    {
        timer.Stop();
        OnStop?.Invoke(timer.ElapsedMilliseconds);
    }
}