using System.Collections.Generic;

namespace XCloud.Platform.Application.Messenger.Connection;

public class ConnectionManager : List<IConnection>, IDisposable
{
    private readonly object _lock = new object();

    public IReadOnlyList<IConnection> AsReadOnlyList()
    {
        lock (this._lock)
        {
            return this.ToList();
        }
    }

    public void AddConnection(IConnection con)
    {
        lock (this._lock)
        {
            this.Add(con);
        }
    }

    public void RemoveConnection(IConnection con)
    {
        lock (this._lock)
        {
            this.Remove(con);
        }
    }

    public void RemoveWhere(Func<IConnection, bool> where)
    {
        lock (this._lock)
        {
            this.RemoveAll(where.Invoke);
        }
    }

    public void RemoveAll()
    {
        lock (this._lock)
        {
            this.Clear();
        }
    }

    public void Dispose()
    {
        foreach (var m in this)
        {
            m.Dispose();
        }

        this.RemoveAll();
    }
}