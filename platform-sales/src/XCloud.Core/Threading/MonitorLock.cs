using System.Threading;

namespace XCloud.Core.Threading;

/// <summary>
/// 异步方法锁不住
/// </summary>
public class MonitorLock : IDisposable
{
    private readonly object _lock;

    public MonitorLock(object _lock)
    {
        this._lock = _lock ?? throw new ArgumentNullException(nameof(_lock));
        if (this._lock.GetType().IsValueType)
            throw new ArgumentNullException("锁对象不能是值类型");

        Monitor.Enter(this._lock);
    }

    public void Dispose()
    {
        Monitor.Exit(this._lock);
    }
}