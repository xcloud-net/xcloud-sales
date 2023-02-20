using System.Threading;

namespace XCloud.Core.Threading;

/// <summary>
/// 使用Semaphore控制并发，SemaphoreSlim不能指定name，不能跨进程process控制并发
/// https://stackoverflow.com/questions/4154480/how-do-i-choose-between-semaphore-and-semaphoreslim
/// </summary>
public class CrossProcessLock : IDisposable
{
    private readonly Semaphore _mutex;

    public CrossProcessLock(string key, TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException(nameof(key));
        this._mutex = new Semaphore(1, 1, key);
        if (!(timeout == null ? this._mutex.WaitOne() : this._mutex.WaitOne(timeout.Value)))
        {
            throw new TimeoutException("wait one returns false");
        }
    }

    public void Dispose()
    {
        this._mutex.Release();
        this._mutex.Dispose();
    }
}