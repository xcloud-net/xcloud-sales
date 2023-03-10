using System.Collections.Generic;
using System.Threading.Tasks;

namespace XCloud.Platform.Common.Application.Service.Messenger.Connection;

public class ClientManager : IDisposable
{
    private readonly List<WsConnection> _connections = new List<WsConnection>();
    private readonly object _lock = new object();

    public IReadOnlyList<WsConnection> AllConnections() => this._connections;

    public void AddConnection(WsConnection con)
    {
        lock (this._lock)
            this._connections.Add(con);
    }

    public void RemoveConnection(WsConnection con)
    {
        lock (this._lock)
            this._connections.Remove(con);
    }

    public void RemoveWhere(Func<WsConnection, bool> where)
    {
        lock (this._lock)
            this._connections.RemoveAll(x => where.Invoke(x));
    }

    public void RemoveAll()
    {
        lock (this._lock)
            this._connections.RemoveAll(x => true);
    }

    public void Dispose()
    {
        var closeTasks = this._connections.Select(x => x.CloseAsync()).ToArray();
        Task.WhenAll(closeTasks).Wait();
        this.RemoveAll();
    }
}