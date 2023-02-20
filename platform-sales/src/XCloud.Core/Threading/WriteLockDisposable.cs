using System.Threading;

namespace XCloud.Core.Threading;

public class WriteLockDisposable : IDisposable
{
    private readonly ReaderWriterLockSlim _rwLock;

    public WriteLockDisposable(ReaderWriterLockSlim rwLock)
    {
        _rwLock = rwLock;
        _rwLock.EnterWriteLock();
    }

    void IDisposable.Dispose()
    {
        _rwLock.ExitWriteLock();
    }
}